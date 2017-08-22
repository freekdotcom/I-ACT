using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;

using Xamarin.Forms;


using ACD.App.Droid;


[assembly: Dependency(typeof(Alert))]

namespace ACD.App.Droid
{
    /// <summary>
    /// Custom class for the alerts
    /// </summary>
    /// <seealso cref="ACD.IAlert" />
    public class Alert : IAlert
    {
        public static readonly int AlertWidth = Device.Idiom == TargetIdiom.Phone ? 270 : 320;

        class AlertDialogFragment : DialogFragment
        {
            public string Title;
            public string Body;
            public View Content;
            public List<AlertButton> Buttons;
            public TaskCompletionSource<object> tsc;


            /// <summary>
            /// Custom Alert builder for the Android app.
            /// Needs to be a custom class
            /// </summary>
            /// <param name="activ">The activity.</param>
            /// <returns></returns>
            public Dialog AndroidCustomAlert(Activity activ)
            {
                Android.Views.LayoutInflater inflater = Android.Views.LayoutInflater.From(activ);
                Android.Views.View view = inflater.Inflate(Resource.Layout.AlertDialogLayout, null);

                AlertDialog.Builder builder = new AlertDialog.Builder(activ);

                TextView title = view.FindViewById<TextView>(Resource.Id.Login);
                title.Text = Title;

                TextView body = view.FindViewById<TextView>(Resource.Id.pincodeText);
                body.Text = Body;
                body.MovementMethod = new Android.Text.Method.ScrollingMovementMethod();

                switch (Title)
                {
                    case "Welkom!":
                        AuthenticationAlert(view, builder, inflater, Buttons);
                        break;

                    case "Hoe was je dag?":
                        HappyAlert(inflater, builder);
                        break;

                    case "Hoe gaat het?":
                        HappyAlert(inflater, builder);
                        break;

                    case "Dagboekje":
                        YesOrNoAlert(inflater, builder);
                        break;

                    case "Tijd bewerken":
                        TimeAlert(inflater, builder);
                        break;

                    case "Tijd toevoegen":
                        TimeAlert(inflater, builder);
                        break;

                    case "Pincode gereset!":
                        NeutralAlert(inflater, builder);
                        break;

                    case "Pincode resetten":
                        YesOrNoAlert(inflater, builder);
                        break;

                    case "Nieuwe tip":
                        YesOrNoAlert(inflater, builder);
                        break;

                    case "Dank je wel!":
                        NeutralAlert(inflater, builder);
                        break;

                    default:
                        break;
                };

                builder.SetCancelable(false);
                return builder.Create();
            }

            public void YesOrNoAlert(Android.Views.LayoutInflater inflater, AlertDialog.Builder builder)
            {
                Android.Views.View secondView = inflater.Inflate(Resource.Layout.YesOrNoLayout, null);
                builder.SetView(secondView);

                TextView title = secondView.FindViewById<TextView>(Resource.Id.Login);
                title.Text = Title;

                TextView body = secondView.FindViewById<TextView>(Resource.Id.pincodeText);
                body.Text = Body;
                body.MovementMethod = new Android.Text.Method.ScrollingMovementMethod();

                Android.Widget.Button btnPositive = secondView.FindViewById<Android.Widget.Button>(Resource.Id.btnLoginLL);
                Android.Widget.Button btnNegative = secondView.FindViewById<Android.Widget.Button>(Resource.Id.btnClearLL);


                //Positive button feedback
                btnPositive.Text = Buttons.Last().Text;
                btnPositive.Click += delegate
                {
                    CommandsForButtons(Buttons.Last());
                };

                //Negative button feedback
                btnNegative.Text = Buttons.First().Text;
                btnNegative.Click += delegate
                {

                    CommandsForButtons(Buttons.First());
                };
            }

            public void AuthenticationAlert(Android.Views.View view, AlertDialog.Builder builder, Android.Views.LayoutInflater inflater,
                                           List<AlertButton> buttons)
            {
                builder.SetView(view);
                EditText pincode = view.FindViewById<EditText>(Resource.Id.pincodeEditText);
                Android.Widget.Button btnPositive = view.FindViewById<Android.Widget.Button>(Resource.Id.btnLoginLL);
                Android.Widget.Button btnNegative = view.FindViewById<Android.Widget.Button>(Resource.Id.btnClearLL);

                if (buttons == null || buttons.Count == 0)
                {
                    NeutralAlert(inflater, builder);
                }


                //Positive button feedback
                btnPositive.Text = Buttons.Last().Text;
                btnPositive.Click += delegate
                {
                    var test = (StackLayout)Content;

                    if (test != null)
                    {
                        var car = (Entry)test.Children[0];
                        car.Text = pincode.Text;
                    }

                    CommandsForButtons(Buttons.Last());
                };

                //Negative button feedback
                btnNegative.Text = Buttons.First().Text;
                btnNegative.Click += delegate
                {
                    CommandsForButtons(Buttons.First());

                };
            }

            public void TimeAlert(Android.Views.LayoutInflater inflater, AlertDialog.Builder builder)
            {
                Android.Views.View secondView = inflater.Inflate(Resource.Layout.TimePickerLayout, null);
                builder.SetView(secondView);
                builder.SetCancelable(false);

                /*TextView title = secondView.FindViewById<TextView>(Resource.Id.Login);
                title.Text = Title;

                TextView body = secondView.FindViewById<TextView>(Resource.Id.pincodeText);
                body.Text = Body;
                body.MovementMethod = new Android.Text.Method.ScrollingMovementMethod(); */

                Android.Widget.Button btnPositive = secondView.FindViewById<Android.Widget.Button>(Resource.Id.btnLoginLL);
                Android.Widget.Button btnNegative = secondView.FindViewById<Android.Widget.Button>(Resource.Id.btnClearLL);
                var tp = secondView.FindViewById<Android.Widget.TimePicker>(Resource.Id.timePicker1);
                tp.SetIs24HourView((Java.Lang.Boolean)true);
                //Positive button feedback
                btnPositive.Text = Buttons.Last().Text;
                btnPositive.Click += delegate
                {
                    var car = (Xamarin.Forms.TimePicker)Content;
                    var ts = new TimeSpan(tp.Hour, tp.Minute, 0);
                    car.Time = ts;
                    CommandsForButtons(Buttons.Last());
                };

                //Negative button feedback
                btnNegative.Text = Buttons.First().Text;
                btnNegative.Click += delegate
                {
                    CommandsForButtons(Buttons.First());
                };
            }

            public void HappyAlert(Android.Views.LayoutInflater inflater, AlertDialog.Builder builder)
            {

                Android.Views.View secondView = inflater.Inflate(Resource.Layout.HappySliderLayout, null);
                builder.SetView(secondView);

                TextView title = secondView.FindViewById<TextView>(Resource.Id.Login);
                title.Text = Title;

                TextView body = secondView.FindViewById<TextView>(Resource.Id.pincodeText);
                body.Text = Body;
                body.MovementMethod = new Android.Text.Method.ScrollingMovementMethod();

                Android.Widget.Button btnNeutral = secondView.FindViewById<Android.Widget.Button>(Resource.Id.btnNeutral);
                ImageView imgHappy = secondView.FindViewById<ImageView>(Resource.Id.imgHappy);
                ImageView imgSad = secondView.FindViewById<ImageView>(Resource.Id.imgSad);


                var happySlider = secondView.FindViewById<SeekBar>(Resource.Id.happinessSlider);

                happySlider.Visibility = Android.Views.ViewStates.Visible;
                btnNeutral.Text = Buttons.First().Text;
                btnNeutral.Click += delegate
                {
                    var car = (StackLayout)Content;

                    var layoutView = (Xamarin.Forms.AbsoluteLayout)car.Children[1];
                    var slider = (Slider)layoutView.Children[1];

                    var totalHappyValue = happySlider.Progress / 10;
                    slider.Value = totalHappyValue;


                    CommandsForButtons(Buttons.First());
                };
            }

            public void NeutralAlert(Android.Views.LayoutInflater inflater, AlertDialog.Builder builder)
            {
                Android.Views.View secondView = inflater.Inflate(Resource.Layout.DefaultAlertLayout, null);
                builder.SetView(secondView);

                TextView title = secondView.FindViewById<TextView>(Resource.Id.Login);
                title.Text = Title;

                TextView body = secondView.FindViewById<TextView>(Resource.Id.pincodeText);
                body.Text = Body;
                body.MovementMethod = new Android.Text.Method.ScrollingMovementMethod();

                Buttons = new List<AlertButton> {
                        new AlertButton {
                            Text = "Oké",
                            IsPreferred = true,
                            Action = () => false
                            }
                        };
                Android.Widget.Button btnNeutral = secondView.FindViewById<Android.Widget.Button>(Resource.Id.btnNeutral);
                btnNeutral.Text = Buttons.First().Text;
                btnNeutral.Click += delegate
                {
                    CommandsForButtons(Buttons.First());
                };
            }

            /// <summary>
            /// Commands for buttons.
            /// </summary>
            /// <param name="button">The button.</param>
            public void CommandsForButtons(AlertButton button)
            {
                Func<Task> dismiss = null;
                var command = new Command(async () =>
                {
                    var ab = button;
                    var cont = true;
                    if (ab.Action != null)
                        cont = ab.Action();
                    if (ab.ActionAsync != null)
                    {
                        cont = cont && await ab.ActionAsync();
                    }
                    if (!cont)
                    {
                        await dismiss();
                    }
                });

                dismiss = async () =>
                {
                    await Task.Run(() =>
                    {
                        Dismiss();
                        tsc.SetResult(null);
                    });

                    Log.Debug("TSC", tsc.Task.Status.ToString());

                };

                command.Execute(this);
            }

            /// <summary>
            /// Called when the dialog is created.
            /// </summary>
            /// <param name="savedInstanceState">State of the saved instance.</param>
            /// <returns></returns>
            public override Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                var test = AndroidCustomAlert(Activity);
                test.SetCanceledOnTouchOutside(false);
                test.SetCancelable(false);


                return test;
            }


        }

        /// <summary>
        /// Shows the specified Alert within the applciation.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="body">The body.</param>
        /// <param name="content">The content.</param>
        /// <param name="buttons">The buttons.</param>
        /// <returns></returns>
        public async Task Show(string title, string body, View content, List<AlertButton> buttons)
        {
            var tcs = new TaskCompletionSource<object>();
            var adf = new AlertDialogFragment
            {
                Title = title,
                Body = body,
                Content = content,
                Buttons = buttons,
                tsc = tcs
            };

            var FragmentManager = ((Activity)Forms.Context).FragmentManager;
            FragmentTransaction ft = FragmentManager.BeginTransaction();

            //Remove fragment else it will crash as it is already added to backstack
            Fragment prev = FragmentManager.FindFragmentByTag("alert");
            if (prev != null)
            {
                ft.Remove(prev);
            }

            ft.AddToBackStack(null);
            adf.Cancelable = false;
            adf.Show(ft, "alert");

            Log.Debug("TSC", tcs.Task.Status.ToString());

            await tcs.Task;

        }


    }

}
