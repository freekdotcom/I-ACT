using ACD.App.Droid;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: ExportRenderer(typeof(TableView), typeof(CustomTableView))]
namespace ACD.App.Droid
{
    /// <summary>
    /// This class is needed in order to create a custom render for the tableview. Without it, some text becomes white.
    /// </summary>
    /// <seealso cref="Xamarin.Forms.Platform.Android.TableViewRenderer" />
    public class CustomTableView : TableViewRenderer
    {
        /// <summary>
        /// Gets the model renderer.
        /// </summary>
        /// <param name="listView">The list view.</param>
        /// <param name="view">The view.</param>
        /// <returns></returns>
        protected override TableViewModelRenderer GetModelRenderer(Android.Widget.ListView listView, TableView view)
        {
            return new CustomTableViewModelRenderer(this.Context, listView, view);
        }
    }
}
