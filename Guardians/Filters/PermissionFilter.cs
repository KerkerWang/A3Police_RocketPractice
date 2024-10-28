using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Xml;

namespace Guardians.Filters
{
    public class PermissionFilter : ActionFilterAttribute
    {

        public string Module { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controllerName = string.IsNullOrEmpty(Module) ? filterContext.HttpContext.GetRouteData().Values["controller"].ToString() : Module;
            string actionName = string.IsNullOrEmpty(Module) ? filterContext.HttpContext.GetRouteData().Values["action"].ToString() : Module;


            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"wwwroot\Config\Menu.xml");

            var permission = filterContext.HttpContext.User.FindFirst(ClaimTypes.Authentication).Value;
            XmlNode rootnode = xmlDoc.DocumentElement;
            string selectControllerName = "";
            string title = "";
            Stack<string> stacks = new Stack<string>();
            if (controllerName != "Home")
            {

                var selectNode = xmlDoc.DocumentElement.SelectSingleNode("//Modules[@Controller='" + controllerName + "' and @Action='Index']");

                string selectValue = selectNode.Attributes["Value"].Value;
                selectControllerName = selectNode.Attributes["Controller"].Value;
                title = selectNode.Attributes["Title"].Value;
                if (filterContext.HttpContext.User == null || permission.ToLower().IndexOf(selectValue.ToLower()) == -1)
                {
                    filterContext.Result = new RedirectResult(@"\Admin\Account\Login");
                    return;
                }
                while (selectNode.Attributes["Title"] != null)
                {
                    stacks.Push(selectNode.Attributes["Title"].Value);
                    selectNode = selectNode.ParentNode;
                }
            }
            else
            {
                stacks.Push("");
            }
            if (filterContext.Controller is Controller controller)
            {
                controller.ViewBag.Breadcrumbs = stacks;
                controller.ViewBag.Menu = GetMenu(permission, selectControllerName, rootnode, filterContext.HttpContext.GetRouteData());
                controller.ViewBag.Title = title;
                controller.ViewBag.UserName = filterContext.HttpContext.User.FindFirst(ClaimTypes.GivenName).Value;
                controller.ViewBag.Action = actionName;
            }
        }

        public string GetMenu(string permission, string selectControllerName, XmlNode rootnode, RouteData url)
        {
            StringBuilder sb = new StringBuilder();
            string layout = $@"<li class='nav-item'>
                <a href = '#'><i class='ft-server'></i><span class='menu-title' data-i18n=''>Menu levels</span></a>
                <ul class='menu-content'>
                    <li><a class='menu-item' href='#'>Second level</a></li>
                    <li><a class='menu-item' href='#'>Second level child</a>
                        <ul class='menu-content'>
                            <li><a class='menu-item' href='#'>Third level</a></li>
                        </ul>
                    </li>
                </ul>
                </li>
                <li class=' nav-item'>
                <a asp-action='index' asp-controller='Member'><i class='ft-life-buoy'></i><span class='menu-title' data-i18n=''>使用者管理</span></a>
                </li>";
            var strClass = "";
            foreach (XmlNode node in rootnode.ChildNodes)
            {
                //判斷是否有權限顯示
                if (node.Attributes != null &&
                    permission.IndexOf(node.Attributes["Value"].Value, System.StringComparison.Ordinal) > -1)
                {
                    //判斷是否使用中
                    strClass = node.Attributes["Controller"].Value.ToLower() == selectControllerName.ToLower() ? "open" : "";
                    sb.Append($"<li class='sidenav-item {strClass}'>");

                    string currentController = url.Values["controller"].ToString();
                    string currentAction = url.Values["action"].ToString();
                    sb.Append($"<a href='/{node.Attributes["Controller"].Value}/{node.Attributes["Action"].Value}' class=\"nav-link \">");
                    sb.Append($"<span class=\"pcoded-micon\"><i class='{node.Attributes["Icon"].Value}'></i></span>");
                    sb.Append($"<span class=\"pcoded-mtext\">{node.Attributes["Title"].Value}</span></a>");
                    if (node.HasChildNodes)
                    {
                        sb.Append("<ul class='sidenav-menu'>");
                        sb.Append(GetSubMenu(permission, selectControllerName, node, url));
                        sb.Append("</ul>");
                    }
                    sb.Append("</li>");
                }
            }
            return sb.ToString();
        }

        public string GetSubMenu(string permission, string selectControllerName, XmlNode rootnode, RouteData url)
        {
            StringBuilder sb = new StringBuilder();
            var strClass = "";

            foreach (XmlNode node in rootnode.ChildNodes)
            {
                //判斷是否有權限顯示
                if (node.Attributes != null &&
                    permission.IndexOf(node.Attributes["Value"].Value, System.StringComparison.Ordinal) > -1)
                {
                    //判斷是否使用中
                    strClass = node.Attributes["Controller"].Value.ToLower() == selectControllerName.ToLower() ? "active" : "";
                    sb.Append($"<li class='sidenav-item {strClass}'>");

                    string currentController = url.Values["controller"].ToString();
                    string currentAction = url.Values["action"].ToString();
                    sb.Append($"<a href='/{node.Attributes["Controller"].Value}/{node.Attributes["Action"].Value}' class=\"nav-link \">");
                    sb.Append($"<span class=\"pcoded-mtext\">{node.Attributes["Title"].Value}</span></a>");
                    if (node.HasChildNodes)
                    {
                        sb.Append("<ul class='sidenav-menu'>");
                        sb.Append(GetSubMenu(permission, selectControllerName, node, url));
                        sb.Append("</ul>");
                    }
                    sb.Append("</li>");
                }
            }
            return sb.ToString();
        }



    }

}

