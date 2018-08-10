using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Serialization;
namespace Web.Helpers.Buybid.Model
{
  
    public class ItemDetails
    {
        [DataMember(Name = "Items")]
        public List<ItemDetail> Items { get; set; }
    }
    public struct ItemDetail
    {
        [DataMember(Name = "Item")]
        public Item Item { get; set; }

    }
   

    public class Product : BaseInfo
    {
        [DataMember(Name = "productId")]
        public string ProductId { get; set; }
        [DataMember(Name = "productName")]
        public string ProductName { get; set; }
        [DataMember(Name = "productNo")]
        public string ProductNo { get; set; }
        [DataMember(Name = "brandName")]
        public string BrandName { get; set; }
        [DataMember(Name = "smallImageUrl")]
        public string SmallImageUrl { get; set; }
        [DataMember(Name = "mediumImageUrl")]
        public string MediumImageUrl { get; set; }
        [DataMember(Name = "productCaption")]
        public string ProductCaption { get; set; }
        [DataMember(Name = "maxPrice")]
        public float MaxPrice { get; set; }
        [DataMember(Name = "salesMaxPrice")]
        public float SalesMaxPrice { get; set; }
        [DataMember(Name = "minPrice")]
        public float MinPrice { get; set; }
        [DataMember(Name = "salesMinPrice")]
        public float SalesMinPrice { get; set; }
        [DataMember(Name = "averagePrice")]
        public float AveragePrice { get; set; }
        [DataMember(Name = "reviewCount")]
        public float ReviewCount { get; set; }
        [DataMember(Name = "reviewUrlPC")]
        public string ReviewUrlPC{ get; set; }
    }
    public class Item : BaseInfo
    {
        [DataMember(Name = "itemName")]
        public string ItemName { get; set; }
        [DataMember(Name = "catchcopy")]
        public string Catchcopy { get; set; }
        [DataMember(Name = "itemCode")]
        public string ItemCode { get; set; }
        [DataMember(Name = "itemPrice")]
        public float ItemPrice { get; set; }
        
        public string DisplayImage
        {
            get
            {
                string img = "/images/no-img.png";
                if (MediumImageUrls != null && MediumImageUrls.Count > 0)
                {
                    img = MediumImageUrls[0].ImageUrl;
                }
                else if (SmallImageUrls != null && SmallImageUrls.Count > 0)
                {
                    img = SmallImageUrls[0].ImageUrl;
                }
                return img;
            }
        }
        [DataMember(Name = "itemCaption")]
        public string ItemCaption { get; set; }
        [DataMember(Name = "smallImageUrls")]
        public List<ImageUrls> SmallImageUrls { get; set; }
        [DataMember(Name = "mediumImageUrls")]
        public List<ImageUrls> MediumImageUrls { get; set; }
        [DataMember(Name = "shopName")]
        public string ShopName { get; set; }
        [DataMember(Name = "shopCode")]
        public string ShopCode { get; set; }
        [DataMember(Name = "shopUrl")]
        public string ShopUrl { get; set; }

        [DataMember(Name = "itemUrl")]
        public string ItemUrl { get; set; }

        [DataMember(Name = "reviewCount")]
        public int ReviewCount { get; set; }
        [DataMember(Name = "pointRate")]
        public int PointRate { get; set; }
        [DataMember(Name = "endTime")]
        public string EndTime { get; set; }
        [DataMember(Name = "startTime")]
        public string StartTime { get; set; }
        [DataMember(Name = "availability")]
        public int Availability { get; set; }
    }
    public class ImageUrls
    {
        
        [DataMember(Name = "imageUrl")]
        public string ImageUrl{get ;set; }
      
        
    }
}