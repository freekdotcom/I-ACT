using System;

using Android.App;
using Android.Support.V4.App;
using Android.Content;
using Android.OS;

using Xamarin.Forms;
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]
[assembly: Dependency(typeof(ACD.App.Droid.AndroidNotificationScheduler))]

namespace ACD.App.Droid
{
    [BroadcastReceiver(Enabled = true, DirectBootAware = false, Exported = false )]
    public class NotificationBroadcastReceiver : BroadcastReceiver
    {
        /// <summary>
        /// This method is called when the BroadcastReceiver is receiving an Intent
        /// broadcast.
        /// </summary>
        /// <param name="context">The Context in which the receiver is running.</param>
        /// <param name="intent">The Intent being received.</param>
        /// <remarks>
        /// <para tool="javadoc-to-mdoc">This method is called when the BroadcastReceiver is receiving an Intent
        /// broadcast.  During this time you can use the other methods on
        /// BroadcastReceiver to view/modify the current result values.  This method
        /// is always called within the main thread of its process, unless you
        /// explicitly asked for it to be scheduled on a different thread using
        /// <c><see cref="M:Android.Content.Context.RegisterReceiver(Android.Content.BroadcastReceiver, Android.Content.IntentFilter, Android.Content.IntentFilter, Android.Content.IntentFilter)" /></c>. When it runs on the main
        /// thread you should
        /// never perform long-running operations in it (there is a timeout of
        /// 10 seconds that the system allows before considering the receiver to
        /// be blocked and a candidate to be killed). You cannot launch a popup dialog
        /// in your implementation of onReceive().
        /// </para>
        /// <para tool="javadoc-to-mdoc">
        ///   <format type="text/html">
        ///     <b>If this BroadcastReceiver was launched through a &lt;receiver&gt; tag,
        /// then the object is no longer alive after returning from this
        /// function.</b>
        ///   </format>  This means you should not perform any operations that
        /// return a result to you asynchronously -- in particular, for interacting
        /// with services, you should use
        /// <c><see cref="M:Android.Content.Context.StartService(Android.Content.Intent)" /></c> instead of
        /// <c><see cref="M:Android.Content.Context.BindService(Android.Content.Intent, Android.Content.IServiceConnection, Android.Content.IServiceConnection)" /></c>.  If you wish
        /// to interact with a service that is already running, you can use
        /// <c><see cref="M:Android.Content.BroadcastReceiver.PeekService(Android.Content.Context, Android.Content.Intent)" /></c>.
        /// </para>
        /// <para tool="javadoc-to-mdoc">The Intent filters used in <c><see cref="M:Android.Content.Context.RegisterReceiver(Android.Content.BroadcastReceiver, Android.Content.IntentFilter)" /></c>
        /// and in application manifests are <i>not</i> guaranteed to be exclusive. They
        /// are hints to the operating system about how to find suitable recipients. It is
        /// possible for senders to force delivery to specific recipients, bypassing filter
        /// resolution.  For this reason, <c><see cref="M:Android.Content.BroadcastReceiver.OnReceive(Android.Content.Context, Android.Content.Intent)" /></c>
        /// implementations should respond only to known actions, ignoring any unexpected
        /// Intents that they may receive.</para>
        /// <para tool="javadoc-to-mdoc">
        ///   <format type="text/html">
        ///     <a href="http://developer.android.com/reference/android/content/BroadcastReceiver.html#onReceive(android.content.Context, android.content.Intent)" target="_blank">[Android Documentation]</a>
        ///   </format>
        /// </para>
        /// </remarks>
        /// <since version="Added in API level 1" />
        public override void OnReceive(Context context, Intent intent)
        {
            //Making sure that we receive notifications even when the app is stopped:
            PowerManager.WakeLock sWakeLock;
            var pm = PowerManager.FromContext(context);
            sWakeLock = pm.NewWakeLock(WakeLockFlags.Partial, "GCM Broadcast Reciever Tag");
            sWakeLock.Acquire();

            //Continue with the creation of the notification
            var n = Serialization.Deserialize<Notification>(intent.GetStringExtra("object"));

            var resultIntent = new Intent(context, typeof(MainActivity));
            resultIntent.PutExtra("open", n.Open);
            resultIntent.PutExtra("data", n.Data);
            resultIntent.SetAction(n.Action);
            resultIntent.PutExtra("action", n.Action);
            resultIntent.PutExtra("id", n.ID.ToString());
            resultIntent.AddFlags(ActivityFlags.NewTask | ActivityFlags.SingleTop);


			if (!string.IsNullOrWhiteSpace(n.Data))
			{
				var sp = context.GetSharedPreferences("ACD", FileCreationMode.Private);
				sp.Edit().PutString("launchData", Serialization.Serialize(n.Data)).Commit();
			}


            var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(context);

            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));
            stackBuilder.AddNextIntent(resultIntent);

            var resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

            var nb = new NotificationCompat.Builder(context)
                .SetContentTitle(n.Title)
                .SetSmallIcon(Resource.Drawable.icon)
                .SetContentText(n.Body).SetExtras(resultIntent.Extras)
                .SetContentIntent(resultPendingIntent)
                .SetStyle(new NotificationCompat.BigTextStyle().BigText(n.Body))
                .SetAutoCancel(true);
            var nm = (NotificationManager)context.GetSystemService(Context.NotificationService);
            nm.Notify(n.ID, nb.Build());

            sWakeLock.Release();
        }
    }

    public class AndroidNotificationScheduler : BaseNotificationScheduler
    {
        public AndroidNotificationScheduler()
        {
            foreach (Notification n in Database.Table<Notification>())
            {
                CancelAlarm(n);
                SetAlarm(n);
            }
        }

        /// <summary>
        /// Creates the intent for the notification.
        /// </summary>
        /// <param name="n">The notification.</param>
        /// <returns></returns>
        Intent CreateIntent(Notification n)
        {
            var intent = new Intent(Forms.Context, typeof(NotificationBroadcastReceiver));
            intent.SetData(Android.Net.Uri.Parse(string.Format("content://{0}", n.ID)));
            intent.PutExtra("object", Serialization.Serialize(n));
            return intent;
        }

        /// <summary>
        /// Sets the alarm for the notification.
        /// </summary>
        /// <param name="n">The notification.</param>
        void SetAlarm(Notification n)
        {
            var alarm = (AlarmManager)Forms.Context.GetSystemService(Context.AlarmService);
            var intent = PendingIntent.GetBroadcast(Forms.Context, 0, CreateIntent(n), 0);
            var time = n.Time;
            if (time < DateTime.Now)
                time = DateTime.Now.Date.AddDays(1).Add(time.TimeOfDay);
            if (n.Repeat > TimeSpan.Zero)
            {
                alarm.SetRepeating(AlarmType.RtcWakeup, Java.Lang.JavaSystem.CurrentTimeMillis() + (long)(n.Time - DateTime.Now).TotalMilliseconds, (long)n.Repeat.TotalMilliseconds, intent);
            }
            else
            {
                alarm.Set(AlarmType.RtcWakeup, Java.Lang.JavaSystem.CurrentTimeMillis() + (long)(n.Time - DateTime.Now).TotalMilliseconds, intent);
            }
        }

        /// <summary>
        /// Cancels the alarm for the Notification.
        /// </summary>
        /// <param name="n">The notification.</param>
        void CancelAlarm(Notification n)
        {
            var alarm = (AlarmManager)Forms.Context.GetSystemService(Context.AlarmService);
            var intent = PendingIntent.GetBroadcast(Forms.Context, 0, CreateIntent(n), 0);
            alarm.Cancel(intent);
        }

        /// <summary>
        /// Schedules the specified notification.
        /// </summary>
        /// <param name="n">The notification.</param>
        public override void Schedule(Notification n)
        {
            base.Schedule(n);

            try
            {
                SetAlarm(n);
            }
            catch (Exception e)
            {
                base.Cancel(n.ID);
                throw e;
            }
        }

        /// <summary>
        /// Cancels the specified notification.
        /// </summary>
        /// <param name="id">The id of the notification.</param>
        public override void Cancel(int id)
        {
            var n = Database.Table<Notification>().Where(nt => nt.ID == id).FirstOrDefault();

            if (n == null)
                return;

            CancelAlarm(n);

            base.Cancel(id);
        }

        /// <summary>
        /// Receives the specified notification intent.
        /// </summary>
        /// <param name="intent">The intent.</param>
        public static void Receive(Intent intent)
        {
            var nID = 0;
            if (int.TryParse((string)intent.GetStringExtra("id"), out nID))
                NotificationCenter.Recieve(nID);
        }
    }
}