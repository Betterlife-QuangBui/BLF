using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Pager
/// </summary>
public class Pager
{
    public Pager(int PageSize, int TotalItems, int GroupPage,string Url)
    {
        this.PageSize = PageSize; this.TotalItems = TotalItems; this.GroupPage = GroupPage;
        this.Url = Url;
    }
    public string Url { get; set; }
    public int PageSize { get; set; }
    public int TotalPage
    {
        get
        {
            return (int)Math.Ceiling(1.0 * TotalItems / PageSize);
        }
    }
    public int GroupPage { get; set; }
    public int PageCurrent
    {
        get
        {
            if (HttpContext.Current.Request["page"] == null)
            {
                return 1;
            }
            else
            {
                try
                {
                    return int.Parse(HttpContext.Current.Request["page"]);
                }
                catch {
                    String url = HttpContext.Current.Request.Url.AbsoluteUri;
                    return 1;
                }
            }
        }
    }

    public int PageSkip
    {
        get
        {
            if (PageCurrent == 1) { return 0; }
            else
            {
                return (PageCurrent * PageSize) - PageSize;
            }
        }
    }

    public int prevPage
    {
        get
        {
            if (PageCurrent == 1) { return 1; }
            else
            {
                return PageCurrent - 1;
            }
        }
    }
    public int nextPage
    {
        get
        {
            if (PageCurrent == TotalPage) { return TotalPage; }
            else
            {
                return PageCurrent + 1;
            }
        }
    }

    public int GroupCount
    {
        get
        {
            return (int)Math.Ceiling(1.0 * TotalPage / GroupPage);
        }
    }

    public int TotalItems { get; set; }

    public String Navigation()
    {
        if (TotalItems == 0)
        {
            return "";
        }
        else
        {
            String nav = "";
            
            int pageNo = (int)Math.Ceiling(1.0 * (PageCurrent / 5)) + 1;
            String FirstPage = "<a href='"+string.Format(Url ,1)+ "' class='paginate_button'><</a>";
            String PrevPage = "<a href='" + string.Format(Url, prevPage) + "' class='paginate_button'><</a>";
            String NextPage = "<a href='" + string.Format(Url, nextPage) + "' class='paginate_button'>></a>";
            String LastPage = "<a href='" + string.Format(Url, TotalPage) + "'  class='paginate_button'>>></a>";
            String format = "<a href='{0}' class=' {2}'>{1}</a>";

            for (int i = (pageNo * GroupPage) - GroupPage; i < (pageNo * GroupPage); i++)
            {
                if (i < TotalPage)
                {
                    nav += String.Format(format, String.Format(Url, (i + 1)), (i + 1), (((i + 1)) == PageCurrent) ? "paginate_active" : "paginate_button");
                }
            }

            return FirstPage + PrevPage + nav + NextPage + LastPage;
        }
    }

    public String NavigationPublic()
    {
        if (TotalItems == 0)
        {
            return "";
        }
        else
        {
            String nav = "";
            int pageNo = (int)Math.Ceiling(1.0 * (PageCurrent / 5)) + 1;
            String FirstPage = "<a href='?page=1&&pageNo=" + pageNo + "' class='link-page'>Đầu</a>";
            String PrevPage = "<a href='?page=" + prevPage + "&&pageNo=" + pageNo + "' class='link-page'>Trước</a>";
            String NextPage = "<a href='?page=" + nextPage + "&&pageNo=" + pageNo + "' class='link-page'>Sau</a>";
            String LastPage = "<a href='?page=" + TotalPage + "&&pageNo=" + GroupCount + "'  class='link-page'>Cuối</a>";
            String format = "<a href='?page={0}&&pageNo=" + pageNo + "' class='{2}'>{1}</a>";

            for (int i = (pageNo * GroupPage) - GroupPage; i < (pageNo * GroupPage); i++)
            {
                if (i < TotalPage)
                {
                    nav += String.Format(format, (i + 1), (i + 1), (((i + 1)) == PageCurrent) ? "current-page" : "link-page");
                }
            }

            return FirstPage + PrevPage + nav + NextPage + LastPage + (" <b style='color:rgb(186, 56, 38);'> &nbsp;(Trang " + PageCurrent + "/" + TotalPage + ")</b> ");
        }
    }

}