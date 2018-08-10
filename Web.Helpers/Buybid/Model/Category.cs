using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Web.Helpers.Buybid.Model
{
    public class LoadedRatuken
    {
        [DataMember(Name = "parents")]
        public List<Parents> Parents { get; set; }
        public Children Parent { get; set; }
        [DataMember(Name = "current")]
        public Current Current { get; set; }
        [DataMember(Name = "brothers")]
        public List<Brothers> Brothers { get; set; }
        [DataMember(Name = "children")]
        public List<Children> Children { get; set; }
        [DataMember(Name = "tagGroups")]
        public List<TagGroup> TagGroups { get; set; }
        [DataMember(Name = "pageCount")]
        public int PageCount { get; set; }
        [DataMember(Name = "count")]
        public int Count { get; set; }
        [DataMember(Name = "hits")]
        public int Hit { get; set; }
        [DataMember(Name = "Products")]
        public List<Items> Items { get; set; }

    }
    public class Parents
    {
        [DataMember(Name = "parent")]
        public Parent Parent { get; set; }
    }
    public class Brothers
    {
        [DataMember(Name = "brother")]
        public Brother brother { get; set; }
    }
    public struct Items
    {
        [DataMember(Name = "Product")]
        public Item Item { get; set; }
    }
    public class Brother : BaseInfo { }
    public class  BaseInfo
    {
        [DataMember(Name = "genreId")]
        public string GenreId { get; set; }
        [DataMember(Name = "genreName")]
        public string GenreName { get; set; }
        [DataMember(Name = "genreLevel")]
        public string GenreLevel { get; set; }
    }

    
    public class Parent: BaseInfo
    {
    }
    public class Current:BaseInfo
    {
    }
    public struct TagGroup
    {

    }
    public class Children
    {
        [DataMember(Name = "child")]
        public Child Child { get; set; }
        //Functions func = new Functions();
        //List<Children> cha;
        //public List<Children> pax { get { return cha= func.GetChildrenNode2(Child.GenreId); } set {  cha=value; } }

    }
   
    public class Child:BaseInfo
    {
     

    }
}