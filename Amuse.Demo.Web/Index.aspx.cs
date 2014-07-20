using System;

namespace Amuse.Demo.Web
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write(System.AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}