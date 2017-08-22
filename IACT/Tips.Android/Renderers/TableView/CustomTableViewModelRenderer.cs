using ACD.App.Droid;
using Android.Content;
using Android.Widget;
using Java.Lang;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TableViewModelRenderer), typeof(CustomTableView))]
namespace ACD.App.Droid
{
    public class CustomTableViewModelRenderer : TableViewModelRenderer
    {
        public CustomTableViewModelRenderer(Context Context, Android.Widget.ListView ListView, TableView view)
            : base(Context, ListView, view)
        { }

        /// <summary>
        /// Gets the view. Creates an new view to change the text color of specific table entries.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="convertView">The convert view.</param>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            var view = base.GetView(position, convertView, parent);
            var element = this.GetCellForPosition(position);
            if (element.GetType() == typeof(TextCell))
            {
                try
                {
                    var text = ((((view as LinearLayout).GetChildAt(0) as LinearLayout).GetChildAt(1) as LinearLayout).GetChildAt(0) as TextView);
                    var divider = (view as LinearLayout).GetChildAt(1);

                    text.SetTextColor(Android.Graphics.Color.Rgb(50, 50, 50));
                    divider.SetBackgroundColor(Android.Graphics.Color.Rgb(50, 150, 150));
                }
                catch (Exception) { }
            }
            return view;
        }
    
    }
}
