var tbCarts = localStorage.getItem('tbCarts') // Retrieve the stored data 
tbCarts = JSON.parse(tbCarts) // Converts string to object 
if (tbCarts == null) // If there is no data, initialize an empty array 
  tbCarts = []

// region -- LocalStorage --
var LocalStorage = {
  set: function (key, value) {
    localStorage.setItem(key, JSON.stringify(value))
  },
  get: function (key) {
    var saved = localStorage.getItem(key)
    saved = JSON.parse(saved)
    return saved
  },
  remove: function (key) {
    if (key) {
      localStorage.removeItem(key)
    }
  }
}
// endregion

var translate_value_bg

/* START xử lý template */

// console.info("START")

var elem = document.createElement('div')
elem.className = '_addon-template'
document.body.insertBefore(elem, document.body.childNodes[0])

document.querySelectorAll('._addon-template')[0].style.display = 'none'

function load_template() {
  var con = document.querySelectorAll('._addon-template')[0]
  var xhr = new XMLHttpRequest()

  xhr.onreadystatechange = function (e) {
    if (xhr.readyState == 4 && xhr.status == 200) {
      con.innerHTML = xhr.responseText
      var icon = document.querySelectorAll('._icon-button-add-to-cart-custom')
      if (icon.length && button_add_to_cart_url) {
        icon[0].style.backgroundImage = "url('" + button_add_to_cart_url + "')"
      }

      document.querySelectorAll('._is_translate')[0].disabled = true

      setTimeout(function () {
        var commonTool = new CommonTool()

        // var er = SessionStorage.get("exchange_rate")
        var er = null
        if (er == null) {
          // Luôn lấy qua background
          chrome.runtime.sendMessage({
            action: 'getExchangeRate',
            url: exchange_rate_url,
            callback: 'afterGetExchangeRate'
          })
        } else {
          // console.info("Lấy tỉ giá ở session storage.")
          exchange_rate = parseFloat(er).toFixed(0)

          if (exchange_rate) {
            document.querySelectorAll('._addon-exchange-text')[0].textContent = exchange_rate + ' đ'
          }
          start()
        }

        document.querySelectorAll('._addon-version')[0].textContent = 'v' + version_tool

        if (link_detail_cart) {
          // document.querySelectorAll("._link-detail-cart")[0].setAttribute("href", link_detail_cart)
        }

        document.querySelectorAll('._is_translate')[0].checked = translate_value_bg == 1
        document.querySelectorAll('._is_translate')[0].disabled = false
      }, 2000)
    }
  }

  xhr.open('GET', chrome.extension.getURL('../template/index.html'), true)
  xhr.setRequestHeader('Content-type', 'text/html')
  xhr.send()
}

chrome.runtime.sendMessage({
  action: 'getTranslateValue',
  callback: 'afterGetTranslateValue'
})

/* END xử lý template */

var site_images_url = 'http://ohayoo.vn/'

var CommonTool = function () {
  /**
   * add disabled to button add cart AddToCart
   */
  this.addDisabledButtonCart = function () {
    $('._addToCart').addClass('hidden')
    $('._processaddToCart').removeClass('hidden')
  }
  this.removeDisabledButtonCart = function () {
    $('._processaddToCart').addClass('hidden')
    $('._addToCart').removeClass('hidden')
  }

  this.DisplaySumShop = function(shop_id,jan_code) {

  }

  this.DisplaySumCart = function () {
    var sum_all_yen = 0
    var sum_all_vnd = 0
    tbCarts = localStorage.getItem('tbCarts')
    tbCarts = JSON.parse(tbCarts) // Converts string to object 
    if (tbCarts == null) // If there is no data, initialize an empty array 
      tbCarts = []
    for (var i = 0; i < tbCarts.length; i++) {
      var shop = tbCarts[i]
      sum_all_yen += parseFloat(shop.sum_yen.replace(/\./g, ""))
      sum_all_vnd += parseFloat(shop.sum_vnd.replace(/\./g, ""))
    }
    document.querySelectorAll('.footer_sum_money_yen')[0].textContent = this.currency_format(sum_all_yen) + " Yen"
    document.querySelectorAll('.footer_sum_money_vnd')[0].textContent = this.currency_format(sum_all_vnd) + " VNĐ"
  }

  this.minimizeTitle = function (title, length) {
    var str = title.substring(0, length)
    var index = str.lastIndexOf(' ')
    str = str.substring(0, index)
    return str
  }

  this.loadOptionCategory = function () {
    if (catalog_scalar_url) {
      // Link lấy danh mục mặc định là lấy qua background
      chrome.runtime.sendMessage({ action: 'getCategory', url: catalog_scalar_url, method: 'POST', callback: 'afterGetCategory' })
    }
  }

  /**
   * get origin site
   * @returns {*}
   */
  this.getOriginSite = function () {
    return window.location.hostname
  }

  this.getHomeLand = function () {
    var url = window.location.href
    if (url.match(/rakuten.com/)) {
      return 'RAKUTEN'
    }
    return null
  }

  this.currency_format = function (num, rounding) {
    if (!$.isNumeric(num)) {
      return num
    }
    if (rounding === null || typeof rounding === 'undefined' || rounding == false) {
      var roundingConfig = 10
      num = Math.ceil(num / roundingConfig) * roundingConfig
    }
    num = num.toString().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, '$1.')

    return (num)
  }

  this.getExchangeRate = function () {
    return exchange_rate
  }

  this.trackError = function (link, error, TrackUrl) {
    var param = 'link=' + link + '&error=' + error + '&tool=bookmarklet'

    $.ajax({
      url: TrackUrl,
      type: 'POST',
      data: param,
      success: function (data) { }
    })
  }

  this.hasClass = function (element, $class) {
    return (element.className).indexOf($class) > -1
  }

  this.resizeImage = function (image) {
    return image.replace(/[0-9]{2,3}[x][0-9]{2,3}/g, '150x150')
  }

  this.getParamsUrl = function (name, link) {
    var l = ''
    if (link) {
      l = link
    } else {
      l = window.location.href
    }
    if (l == '') return null

    var results = new RegExp('[\\?&]' + name + '=([^&#]*)').exec(l)
    if (results === null) return null

    return results[1] || 0
  }
  this.processPrice = function (price, site) {
    if (price == null || parseFloat(price) == 0)
      return 0
    var p = 0
    if (price.constructor === Array) {
      p = String(price[0]).replace(',', '').match(/[0-9]*[\.]?[0-9]+/g)
    } else {
      p = String(price).replace(',', '').match(/[0-9]*[\.]?[0-9]+/g)
    }
    if (isNaN(p) || parseFloat(price) == 0) {
      return 0
    }

    var price_show = ''
    var pri = 0
    if (p.constructor === Array && p.length > 1) {
      var pri_start = this.currency_format(p[0] * this.getExchangeRate())
      var key_end = price.length - 1
      var pri_end = this.currency_format(p[key_end] * this.getExchangeRate())

      if (parseFloat(price[key_end]) > 0) {
        price_show = pri_start + ' ~ ' + pri_end
      } else {
        price_show = pri_start
      }
    } else {
      pri = p
      price_show = this.currency_format(pri * this.getExchangeRate())
    }
    var li = document.createElement('li')
    var li_price = null
    var J_PromoPrice = null
    var J_StrPriceModBox = null
    var object = new factory(cart_url, add_to_cart_url)

    if (site == 'RAKUTEN' && translate_value_bg == 1) {
      J_PromoPrice = document.getElementById('rakutenLimitedId_cart')
      var tax_postage1 = object.getTaxPostage1()
      var tax_postage2 = object.getTaxPostage2()
      tax_value1 = this.ajaxTranslate(tax_postage1)
      tax_value2 = this.ajaxTranslate(tax_postage2)
      li.setAttribute('style', 'color: blue ! important; padding: 30px 0px; font-family: arial;')

      li_price = '<span class="ohayoo-append ohayoo-small" style="">Giá</span>     ' +
        '<strong id="price_vnd" class="" style="">' +
        '<b class="ohayoo-append ohayoo-large"  style=""> ' + price_show + ' VNĐ</b><span class="ohayoo-append ohayoo-small" style="">' + tax_value1 + ' ' + tax_value2 + '</span></strong>'
      li.innerHTML = li_price

      if (J_PromoPrice != null || J_PromoPrice.length != 0) {
        J_PromoPrice.parentNode.insertBefore(li, J_PromoPrice.nextSibling)
      }
    }
    return p
  }

  this.timeOut = 0

  this.sendAjaxToCart = function (add_cart_url, data) {
    setTimeout(function () {
      // Luon gui qua background
      chrome.runtime.sendMessage({ action: 'addToCart', url: add_cart_url, data: data, method: 'POST', callback: 'afterAddToCart' })
    }, this.timeOut * 1000)
    this.timeOut++
  }

  this.addProToCart = function (data) {
    var shop_id = data.shop_id
    var checkall = 'checked'
    var flag = false, flagShop = false
    var shippingJ_fee_yen = parseFloat(this.ajaxShippingFee(data.shop_code));
    var shippingJ_fee_vnd = shippingJ_fee_yen * exchange_rate;
    tbCarts = localStorage.getItem('tbCarts') // Retrieve the stored data 
    tbCarts = JSON.parse(tbCarts) // Converts string to object 
    if (tbCarts == null) // If there is no data, initialize an empty array 
      tbCarts = []
    for (var i = 0; i < tbCarts.length; i++) {
      // lay json
      var item = tbCarts[i]

      // kiem tra doi tuong can them co phai da co shop roi hay chua
      if (item.key == shop_id) {
        flagShop = true
        var arrShop = item.value
        var sum_quantity = 0, sum_yen = 0, sum_vnd = 0

        for (var j = 0; j < arrShop.length; j++) {
          var shopItem = arrShop[j]
          if (shopItem.jan_code == data.jan_code) {
            data.quantity = parseFloat(data.quantity) + parseFloat(shopItem.quantity)
            // data.yen_ = parseFloat(data.quantity) * parseFloat(data.price)
            // data.vnd_ = parseFloat(data.price) * parseFloat(data.quantity) * exchange_rate
            data.yen = this.currency_format(parseFloat(data.quantity) * parseFloat(data.price))
            data.vnd = this.currency_format(parseFloat(data.price) * parseFloat(data.quantity) * exchange_rate)
            if (shopItem.checked == '') {
              data.checked = ''
            }
            
            arrShop[j] = data
            tbCarts[i].value = arrShop
            flag = true
          }
          if (shopItem.checked == 'checked') {
              sum_quantity += parseFloat(arrShop[j].quantity)
              sum_yen += parseFloat(arrShop[j].yen.replace(/\./g, ""))
              sum_vnd += parseFloat(arrShop[j].vnd.replace(/\./g, ""))
            }
        }

        // sum_yen += shippingJ_fee_yen;
        // sum_vnd += shippingJ_fee_vnd;
        if (flag == false) {
          arrShop.push(data)
          sum_quantity += parseFloat(data.quantity)
          sum_yen += parseFloat(data.yen.replace(/\./g, ""))
          sum_vnd += parseFloat(data.vnd.replace(/\./g, ""))
          tbCarts[i].value = arrShop
          // sum_yen += shippingJ_fee_yen;
          // sum_vnd += shippingJ_fee_vnd;
        }
        tbCarts[i].time = getDateTime()
        tbCarts[i].sum_quantity = sum_quantity
        tbCarts[i].sum_yen = this.currency_format(sum_yen)
        tbCarts[i].sum_vnd = this.currency_format(sum_vnd)
      }
    }

    if (flagShop == false) {
      var tbShops = []
      tbShops.push(data)      
      // var sum_yen = data.yen_ + shippingJ_fee_yen;
      // var sum_vnd = data.yen_ + shippingJ_fee_vnd;
      var jsonShop = { 'checkall': checkall, 'shippingJ_fee_yen': this.currency_format(shippingJ_fee_yen), 'shippingJ_fee_vnd': this.currency_format(shippingJ_fee_vnd), 'key': data.shop_id, 'shop_code': data.shop_code, 'shop_name': data.shop_name, 'shop_url': data.shop_url, 'time': getDateTime(), 'sum_quantity': data.quantity, 'sum_yen': data.yen, 'sum_vnd': data.vnd, 'value': tbShops }
      tbCarts.push(jsonShop)
      localStorage.setItem('tbCarts', JSON.stringify(tbCarts))
    }
    localStorage.setItem('tbCarts', JSON.stringify(tbCarts))
  }

  this.loadJsFile = function (jsUrl) {
    var file_ali = document.createElement('script')
    file_ali.setAttribute('src', jsUrl + '?t=' + Math.random())
    document.body.appendChild(file_ali)
    return true
  }
  this.key_translate_lib = function (key) {
    var translate = []
    // translate['颜色'] = 'Màu'
    // translate['尺码'] = 'Kích cỡ'
    // translate['尺寸'] = 'Kích cỡ'

    // translate['价格'] = 'Giá'
    // translate['促销'] = 'Khuyến mại'
    // translate['配送'] = 'Vận Chuyển'
    // translate['数量'] = 'Số Lượng'
    // translate['销量'] = 'Chính sách'
    // translate['评价'] = 'Đánh Giá'
    // translate['颜色分类'] = 'Màu sắc'
    // translate['促销价'] = 'Giá'

    // translate['套餐类型'] = 'Loại'
    // translate['单价（元）'] = 'Giá (NDT)'
    // translate['库存量'] = 'Tồn kho'
    // translate['采购量'] = 'SL mua'

    // translate['材质保障'] = "Chất lượng"
    // translate['15天包换'] = "15 đổi trả"
    // translate['48小时发货'] = "48 giờ giao hàng"

    translate['価格'] = 'Giá'
    translate['プロモーション'] = 'Khuyến mãi'
    translate['商品番号'] = 'Mã số'
    translate['税込'] = 'đã bao gồm thuế'

    var detect = key
    if (translate[key]) {
      detect = translate[key]
    }
    return detect
  }
  this.stripTags = function (object) {
    if (typeof object == 'object') {
      return object.replaceWith(object.html().replace(/<\/?[^>]+>/gi, ''))
    }
    return false
  }

  this.setCategorySelected = function (category_id) {
    this.setCookie('category_selected', category_id, 100)
    return true
  }

  this.getCategorySelected = function () {
    return this.getCookie('category_selected')
  }

  this.translate_guarantee_type = function () {
    var process = false

    if (isUsingSetting) { // Nếu sử dụng setting thì lấy giá trị ở setting
      if (isTranslate) {
        process = true
      }
    } else { // Nếu ko sử dụng setting thì lấy giá trị ở cookie
      if (translate_value_bg == 1) {
        process = true
      }
    }

    try {
      if (process) {
        var list = document.querySelectorAll('ul.guarantee-type > li')
        for (var i = 0; i < list.length; i++) {
          if (i < 3) {
            var text = list[i].getElementsByTagName('div')[0].getElementsByTagName('a')[0].textContent.trim()
            if (text) {
              var text_translate = this.key_translate_lib(text)
              if (text_translate) {
                list[i].getElementsByTagName('div')[0].getElementsByTagName('a')[0].textContent = text_translate
              }
            }
          }
        }
      }
    } catch (e) {
      // console.info("Có lỗi xảy ra khi dịch guarantee_type")
      // console.warn(e.message)
    }
  }

  this.translate_title = function (title, type, object) {
    if (isUsingSetting) { // Nếu sử dụng setting thì lấy giá trị ở setting
      if (isTranslate) {
        // if(site_using_https){
        //    Action.request({
        //        url: translate_url,
        //        type: 'POST',
        //        data: { text:title, type:type }
        //    }).done(function (response) {
        //        Action.afterTranslate({ response : response })
        //    })
        // }else{
        //    chrome.runtime.sendMessage({ action: "translate", url: translate_url, method: 'POST', data: { text:title, type:type }, callback: 'afterTranslate'
        //
        //    })
        // }

        // Luôn gửi qua background
        chrome.runtime.sendMessage({
          action: 'translate', url: translate_url, method: 'POST', data: { text: title }, callback: 'afterTranslate'
        })
        return true
      }
    } else {
      console.info('translate_value_bg: ' + translate_value_bg)
      if (translate_value_bg == 1) {
        // if(site_using_https){
        //    Action.request({
        //        url: translate_url,
        //        type: 'POST',
        //        data: { text:title, type:type }
        //    }).done(function (response) {
        //        Action.afterTranslate({ response : response })
        //    })
        // }else{
        //    chrome.runtime.sendMessage({ action: "translate", url: translate_url, method: 'POST', data: { text:title, type:type }, callback: 'afterTranslate'
        //
        //    })
        // }
        // Luôn gửi qua background
        chrome.runtime.sendMessage({
          action: 'translate', url: translate_url, method: 'POST', data: { text: title }, callback: 'afterTranslate'

        })

        return true
      }
    }

    return false
  }

  this.translate = function (dom, type) {
    if (isUsingSetting) { // Nếu sử dụng setting thì lấy giá trị ở setting
      if (isTranslate && type == 'properties') {
        this.translateStorage(dom)
      }
    } else { // Nếu ko sử dụng setting thì lấy giá trị ở cookie
      if (translate_value_bg == 1) {
        if (type == 'properties') {
          this.translateStorage(dom)
        }
      }
    }
  }

  this.translateStorage = function (dom) {
    // write by vanhs | edit_time: 13/06/2015
    try {
      var content
      try { // for jquery
        content = dom.text()
      } catch (m) { // for javascript
        content = dom.textContent
      }

      var content_origin = content
      var resource = keyword
      if (resource != null) {
        var data = resource.resource

        for (var i = 0; i < data.length; i++) {
          var obj = data[i]
          try {
            if (content.match(obj.k_c, 'g')) {
              content = content.replace(obj.k_c, obj.k_v + ' ')
            }
          } catch (ex) {
            try {
              if (content.match(obj.keyword_china, 'g')) {
                content = content.replace(obj.keyword_china, obj.keyword_vi + ' ')
              }
            } catch (ex) { }
          }
        }
        try { // for jquery
          dom.text(content)
          dom.attr('data-text', content_origin)
        } catch (k) {
          dom.innerHTML = content
          dom.setAttribute('data-text', content_origin)
        }
      }
    } catch (e) {
      console.log('error: ' + e.message)
    }
  }

  this.ajaxTranslate = function (title) {
    // var context = dom.text()
    var data = ''

    $.ajax({
      url: translate_microsoft_url,
      type: 'POST',
      data: JSON.stringify({
        'text': title,
      }),
      contentType: 'application/json',
      dataType: 'json',
      async: false,

      success: function (result) {
        data = result.title
      }
    })
    return data
  }

  this.ajaxShippingFee = function (shop_code) {
    // var context = dom.text()
    var data = ''

    $.ajax({
      url: shippingJ_fee_url,
      type: 'POST',
      data: JSON.stringify({
        'shopCode': shop_code,
      }),
      contentType: 'application/json',
      dataType: 'json',
      async: false,

      success: function (result) {
        data = result.price
      }
    })
    return data
  }

  this.getKeywordSearch = function () {
    $.ajax({
      url: translate_keyword_url,
      type: 'POST',
      contentType: 'application/json',
      xhrFields: {
        withCredentials: true
      },
      data: {
        text: 'text',
        type: 'type'
      },

      success: function (data) {
        var resource = JSON.stringify(data)
        localStorage.setItem('keyword_search', resource)
      }
    })
    return true
  }

  /* Hien thi input khi xảy ra lỗi lấy dữ liệu*/
  this.showInputEx = function (site) {
    $('.frm-tool').hide()
    $('li#li_sd_price').fadeIn()
    var box_input_exception = $('#_box_input_exception')
    box_input_exception.show()
    box_input_exception.attr('data-is-show', 1)

    var price_dom = $('#_price')

    var object = new factory(cart_url, add_to_cart_url)
    var price_origin = object.getPriceInput()
    var properties_origin = object.getPropertiesInput()
    var quantity = object.getQuantityInput()
    if (quantity == '' && properties_origin == '' && price_origin == '') {
      alert('Chúng tôi không thể lấy được thông tin của sản phẩm.' +
        'Bạn vui lòng điền thông tin để chúng tôi mua hàng cho bạn')
      price_dom.focus()
      $('._close_tool').click()

      try {
        if (site != 'alibaba') {
          if (parseFloat(object.getPromotionPrice()) > 0) {
            price_dom.val(object.getPromotionPrice())
          } else {
            price_dom.attr('placeholder', 'Nhập tiền tệ - Nhật Bản')
          }
          $('#_properties').val(object.getPropertiesOrigin())
          $('#_quantity').val(object.getQuantity())
        }
      } catch (ex) {
        console.log(ex)
      }
    }
    return true
  }

  this.setCookie = function (cname, cvalue, exdays) {
    var d = new Date()
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000))
    var expires = 'expires=' + d.toGMTString()
    document.cookie = cname + '=' + cvalue + '; ' + expires
    return true
  }

  this.getCookie = function (cname) {
    var name = cname + '='
    var ca = document.cookie.split(';')
    for (var i = 0; i < ca.length; i++) {
      var c = ca[i]
      while (c.charAt(0) == ' ') c = c.substring(1)
      if (c.indexOf(name) != -1) {
        return c.substring(name.length, c.length)
      }
    }
    return ''
  }
}

var factory = function (cart_url, add_cart_url) {
  var _class

  var url = window.location.href
  if (url.match(/rakuten.co.jp/)) {
    _class = new rakuten(cart_url)
  }
  if (url.match(/amazon.co.jp/)) {
    _class = new amazon(cart_url)
  }
  return _class
}

/**
 * Created by Admin on 9/19/14.
 */
var AddonTool = function () {
  /**
   * item_data: Array
   * keys: amount, color, size
   */
  this.common_tool = new CommonTool()

  /* Cho vao gio hang doi voi rakuten */
  this.AddToCart = function () {
    var error = 0

    var object = new factory(cart_url, add_to_cart_url)

    var jan_code = '',
      title_origin = '',
      link_origin = '',
      quantity = '',
      price_promotion = '',
      price_origin = '',
      size = '',
      color = '',
      desc = '',
      img_src = '',
      site_name = '',
      item_id = '',
      item_code = '',
      shop_id = '',
      shop_name = '',
      shop_code = '',
      shop_url = '',
      checked = 'checked'

    var select_category = $('._select_category')
    var loaded_category = select_category.attr('data-loaded')

    var category_id = select_category.val()

    var category_name = $('._select_category option:selected').text()

    var brand = $('._brand_item').val()

    while (category_name.match(/-/i)) {
      category_name = category_name.replace(/-/i, '')
    }

    if (category_id === '-1') {
      category_name = $('._input_category').val()
    }

    if ((category_id === '0' || category_name == '') && loaded_category === '1') {
      alert('Yêu cầu chọn danh mục cho sản phẩm')
      this.common_tool.removeDisabledButtonCart()
      return false
    }

    var check_select = object.checkSelectFull()

    if (!check_select) {
      alert('Yêu cầu chọn đầy đủ thuộc tính của sản phẩm')
      this.common_tool.removeDisabledButtonCart()
      return false
    }

    jan_code = object.getJanCode()
    title_origin = object.getTitleOrigin()
    link_origin = window.location.href
    img_src = object.getImgLink()
    site_name = object.getSiteName()
    item_id = object.getItemId()
    item_code = object.getItemCode()
    shop_id = object.getShopId()
    shop_name = object.getShopName()
    shop_code = object.getShopCode()
    shop_url = object.getShopUrl()

    // price_origin = object.getOriginPrice()
    price_promotion = object.getPromotionPrice()

    // if($.isArray(price_origin)){
    // price_origin = price_origin[0]
    // }

    if ($.isArray(price_promotion)) {
      price_promotion = price_promotion[0]
    }

    quantity = object.getQuantity()

    var data = {
      jan_code: $.trim(jan_code),
      title_origin: $.trim(title_origin),
      mini_title: common_tool.minimizeTitle($.trim(title_origin), 30),
      price: price_promotion,
      price_origin: parseFloat(price_promotion.replace(/\./g, "")),
      price_promotion: common_tool.currency_format(price_promotion),
      quantity: parseFloat(quantity),
      // yen_: parseFloat(quantity * price_promotion),
      // vnd_: parseFloat(quantity * price_promotion * exchange_rate),
      yen: common_tool.currency_format(quantity * price_promotion),
      vnd: common_tool.currency_format(quantity * price_promotion * exchange_rate),
      link_origin: link_origin,
      error: error,
      weight: 0,
      step: 1,
      brand: brand,
      category_name: category_name,
      category_id: category_id,
      size: size,
      color: color,
      desc: desc,
      img_src: img_src,
      site_name: site_name,
      item_id: item_id,
      item_code: item_code,
      shop_id: shop_id,
      shop_name: shop_name,
      shop_code: shop_code,
      shop_url: shop_url,
      checked: checked,
      tool: 'Addon',
      version: version_tool,
      time: getDateTime()
    }

    common_tool.addProToCart(data)
    alert('Đã thêm sản phẩm vào giỏ hàng!')
    common_tool.removeDisabledButtonCart()
    // this.common_tool.sendAjaxToCart(add_to_cart_url,data)
  }
}

var common_tool = new CommonTool()
var origin_site = common_tool.getOriginSite()
var addon_tool = new AddonTool()

var SessionStorage = {
  set: function (key, value) {
    window.sessionStorage.setItem(key, JSON.stringify(value))
  },
  get: function (key) {
    var saved = window.sessionStorage.getItem(key)
    saved = JSON.parse(saved)
    return saved
  }
}

var Action = {
  afterGetExchangeRate: function (request) {
    console.info('afterGetExchangeRate')
    if (request.response) {
      exchange_rate = parseFloat(request.response).toFixed(2)
      SessionStorage.set('exchange_rate', exchange_rate)
    } else {
      exchange_rate = '219.27'
    }
    if (exchange_rate) {
      $('._addon-exchange-text').text(exchange_rate + ' đ')
    }
    start()
  },

  afterAddToFavorite: function (request) {
    console.info('afterAddToFavorite')
    alert('Lưu sản phẩm thành công!')
  },

  afterTranslate: function (request) {
    console.info('afterTranslate')
    try {
      var object = new factory(cart_url, add_to_cart_url)
      var result = request.response.title
      object.set_translate({ title: result })
    } catch (ex) {
      console.warn(ex.message)
    }
  },

  afterGetCategory: function (request) {
    console.info('afterGetCategory')
    var data = request.response
    var option = '<option value="0">Chọn danh mục</option>'

    var ct = new CommonTool()
    var category_id = ct.getCategorySelected()

    for (var i = 0; i < data.length; i++) {
      var catalog = data[i]
      option += '<option value="' + catalog.id + '"'
      if (parseInt(category_id) === parseInt(catalog.id)) {
        option += ' selected="selected"'
      }
      option += '>'
      for (var j = 0; j < catalog.level; j++) {
        if (parseInt(catalog.level) > 1) {
          option += '&#8212;'
        }
      }
      option += catalog.name + '</option>'
    }
    option += '<option value="-1">Khác</option>'

    $('._select_category').html(option)
    $('._select_category').attr('data-loaded', 1) // loaded
  },

  afterAddToCart: function (request) {
    console.info('afterAddToCart')

    if (request.response) {
      var common_tool = new CommonTool()
      common_tool.removeDisabledButtonCart()
      if (request.response.html) {
        $('body').append(request.response.html)
      } else {
        $('body').append(request.response)
      }
    } else {
      alert('Không kết nối được tới máy chủ, xin quý khách thử lại sau')
      return
    }
  },

  request: function (params) {
    return $.ajax({
      contentType: 'application/x-www-form-urlencoded',
      xhrFields: {
        withCredentials: true
      },
      headers: { 'X-Requested-With': 'XMLHttpRequest' },

      url: params.url,
      type: params.type == undefined ? 'GET' : params.type,
      data: params.data == undefined ? {} : params.data
    })
  }
}

chrome.runtime.onMessage.addListener(
  function (request, sender, sendResponse) {
    switch (request.action) {
      case 'afterGetExchangeRate':
        Action.afterGetExchangeRate(request)
        break

      case 'afterAddToCart':
        Action.afterAddToCart(request)
        break

      case 'afterGetCategory':
        Action.afterGetCategory(request)
        break

      case 'afterTranslate':
        Action.afterTranslate(request)
        break

      case 'afterAddToFavorite':
        Action.afterAddToFavorite(request)
        break
      case 'afterSetTranslateValue':
        window.location.reload()
        break
      case 'afterGetTranslateValue':
        console.info('afterGetTranslateValue')
        translate_value_bg = request.value
        load_template()
        break

      default:
        break

    }
  }
)

function start() {
  var str = window.location.href
  if (!(str.match(/item.rakuten.co.jp/)
    || str.match(/.amazon.co.jp/))) {
    return false
  }

  document.querySelectorAll('._addon-template')[0].style.display = 'block'

  var object = new factory(cart_url, add_to_cart_url)
  object.init()

  var common = new CommonTool()

  $(document).on('change', '._select_category', function () {
    var catalog_id = $(this).val()
    $('._select_category').val(catalog_id)
    common_tool.setCategorySelected(catalog_id)
    var input_cate = $('._input_category')
    var $panel_category_other = $('._category-other')
    if (catalog_id === '-1') {
      $panel_category_other.removeClass('hidden')
      input_cate.show()
      input_cate.focus()
    } else {
      $panel_category_other.addClass('hidden')
      input_cate.hide()
    }
  })

  $(document).on('keyup', '._brand_item', function () {
    var brand = $(this).val()
    $('._brand_item').val(brand)
  })

  $(document).on('keyup', '._comment_item', function () {
    var comment = $(this).val()
    $('._comment_item').val(comment)
  })

  $(document).on('keyup', '._input_category', function () {
    var category = $(this).val()
    $('._input_category').val(category)
  })

  $(document).on('click', '._addToCart', function () {
    common_tool.addDisabledButtonCart()
    var object = new factory(cart_url, add_to_cart_url)    
    // if (origin_site.match(/1688.com/)) {
    //     object.add_to_cart()
    // } else {
    addon_tool.AddToCart()
    // }
  })
  $(document).on('click', '._is_translate', function () {
    var value = $(this).is(':checked') ? 1 : 0
    chrome.runtime.sendMessage({
      action: 'setTranslateValue',
      value: value,
      callback: 'afterSetTranslateValue'
    })
  })

  $(document).on('click', '._close-warning-ao', function () {
    $('._alert-shop-credible').remove()
  })

  $('._close_tool').click(function () {
    // $('._addon-wrapper').hide()
    // $("._div-block-price-book").fadeIn()
  })

  $('._minimize_tool').click(function () {
    // $('._addon-wrapper').fadeIn()
    // $("._div-block-price-book").hide()
  })

  $('#txt-category').change(function () {
    var value = $(this).val()
    if (parseInt(value) == -1) {
      $('.category-other').show()
      $('.category-other input').focus()
    } else {
      $('.category-other').hide()
    }
  })

  $(document).on('click', '#_add-to-favorite', function () {
    var site = common.getHomeLand()
    var title = site == '1688' ? object.getItemTitle() : object.getTitleOrigin()
    var avatar = site == '1688' ? object.getItemImage() : object.getImgLink()
    var item_id = site == '1688' ? object.getItemId() : object.getItemID()
    var price_promotion = 0
    var price_origin = 0

    if (site == '1688') {
      try {
        var scripts = document.querySelectorAll('script')
        for (var i = 0; i < scripts.length; i++) {
          var html = scripts[i].textContent
          var res = html.search('iDetailConfig')
          if (res != -1) {
            eval(html)

            price_promotion = iDetailConfig.refPrice
            price_origin = iDetailConfig.refPrice

            break
          }
        }
      } catch (e) {
        console.warn(e.message)
      }
    } else {
      price_origin = object.getOriginPrice()
      price_promotion = object.getPromotionPrice()
    }

    var data = {
      avatar: avatar ? decodeURIComponent(avatar) : '',
      item_id: item_id,
      link: window.location.href,
      site: site,
      title: title,
      price: price_promotion > 0 ? price_promotion : price_origin
    }

    // data = JSON.stringify(data)

    if (site_using_https) {
      Action.request({
        url: add_to_favorite_url,
        type: 'POST',
        data: { send_data: data }
      }).done(function (response) {
        Action.afterAddToFavorite({ response: response })
      })
    } else {
      chrome.runtime.sendMessage({
        action: 'addToFavorite',
        url: add_to_favorite_url,
        // url: "http://localhost/ohayoo/www_html/customer/favoriteLink/saveLink",
        data: { send_data: data },
        method: 'POST',
        callback: 'afterAddToFavorite'
      })
    }
  })

  return true
}

function getDateTime() {
  var now = new Date()
  var year = now.getFullYear()
  var month = now.getMonth() + 1
  var day = now.getDate()
  var hour = now.getHours()
  var minute = now.getMinutes()
  var second = now.getSeconds()
  if (month.toString().length == 1) {
    var month = '0' + month
  }
  if (day.toString().length == 1) {
    var day = '0' + day
  }
  if (hour.toString().length == 1) {
    var hour = '0' + hour
  }
  if (minute.toString().length == 1) {
    var minute = '0' + minute
  }
  if (second.toString().length == 1) {
    var second = '0' + second
  }
  var dateTime = year + '-' + month + '-' + day + ' ' + hour + ':' + minute + ':' + second
  return dateTime
}

$(document).on('click', '#myCart', function () {
  $('#tbl').prev().prev().prev().hide()
  $('#rakutenLimitedId_header').hide()
  $('html').css('overflow', 'hidden')
  var modal = document.getElementById('myCartInfo')
  var common = new CommonTool()
  document.querySelectorAll('.ex_rate_money')[0].textContent = exchange_rate + ' VNĐ'
  var template = $('#tutorial-template').html()
  $('.wrapper').html('')
  tbCarts = localStorage.getItem('tbCarts') // Retrieve the stored data 
  tbCarts = JSON.parse(tbCarts) // Converts string to object 
  if (tbCarts == null) // If there is no data, initialize an empty array 
    tbCarts = []
  var arrObj = _.sortByDesc(tbCarts, "time");
  for (var i = 0; i < arrObj.length; i++) {
    var item = arrObj[i];
    var html = Mustache.to_html(template, item).replace(/^\s*/mg, '')
    $('.wrapper').append(html)
  }
  common.DisplaySumCart()
  var span = document.getElementsByClassName('close')[0]
  modal.style.display = 'block'
})

$(document).on('click', '.close', function () {
  var modal = document.getElementById('myCartInfo')
  modal.style.display = 'none'
  $('#tbl').prev().prev().prev().show()
  $('#rakutenLimitedId_header').show()
  $('html').css('overflow', 'auto')
})

$(document).on('change', '.cart_info_checkall', function () {
  var checkboxes = $(this).parents('.cart_info').find('.cart_info_checkone')
  if ($(this).is(':checked')) {
    checkboxes.prop('checked', !checkboxes.prop('')).change();
  } else {
    checkboxes.prop('checked', !checkboxes.prop('checked')).change();
  }
})

$(document).on('change', '.cart_line', function () {
  var common = new CommonTool()
  var shop_id = $(this).attr('data-key')
  var jan_code = $(this).attr('data-jancode')
  var sum_quantity = 0
  var sum_yen = 0
  var sum_vnd = 0
  var checkall = $(this).parents('.cart_info').find('.cart_info_checkall')
  var checkboxes = checkall.parents('.cart_info').find('.cart_info_checkone')
  
  tbCarts = localStorage.getItem('tbCarts')
  tbCarts = JSON.parse(tbCarts) // Converts string to object 
  if (tbCarts == null) // If there is no data, initialize an empty array 
    tbCarts = []
  for (var i = 0; i < tbCarts.length; i++) {
    var shop = tbCarts[i]
    // var shippingJ_fee_yen = parseFloat(common.ajaxShippingFee(shop.shop_code));
    // var shippingJ_fee_vnd = shippingJ_fee_yen * exchange_rate;
    if (shop.key == shop_id) {
    
      var shopItem = shop.value
      for (var j = 0; j < shopItem.length; j++) {
        var item = shopItem[j]
        if (item.jan_code == jan_code && $(this).find('.cart_info_checkone').is(':checked')) {
          item.checked = 'checked'
          item.quantity = $(this).find('.quantity').val()
          item.size = $(this).find('.size').val()
          item.color = $(this).find('.color').val()
          item.desc = $(this).find('.desc').val()
          // item.yen_ = parseFloat(item.quantity) * parseFloat(item.price)
          // item.vnd_ = parseFloat(item.price) * parseFloat(item.quantity) * exchange_rate
          item.yen = common.currency_format(parseFloat(item.quantity) * parseFloat(item.price))
          item.vnd = common.currency_format(parseFloat(item.price) * parseFloat(item.quantity) * exchange_rate)
          shopItem[j] = item
          tbCarts[i].value = shopItem
          $(this).find('.yen').html(item.yen)
          $(this).find('.vnd').html(item.vnd)
          localStorage.setItem('tbCarts', JSON.stringify(tbCarts))
        }
        else if (item.jan_code == jan_code && $(this).find('.cart_info_checkone').not(':checked')) {
          item.checked = ''
          item.quantity = $(this).find('.quantity').val()
          item.size = $(this).find('.size').val()
          item.color = $(this).find('.color').val()
          item.desc = $(this).find('.desc').val()
          // item.yen_ = parseFloat(item.quantity) * parseFloat(item.price)
          // item.vnd_ = parseFloat(item.price) * parseFloat(item.quantity) * exchange_rate
          item.yen = common.currency_format(parseFloat(item.quantity) * parseFloat(item.price))
          item.vnd = common.currency_format(parseFloat(item.price) * parseFloat(item.quantity) * exchange_rate)
          shopItem[j] = item
          tbCarts[i].value = shopItem
          $(this).find('.yen').html(item.yen)
          $(this).find('.vnd').html(item.vnd)
          localStorage.setItem('tbCarts', JSON.stringify(tbCarts))
        }
        sum_quantity += parseFloat(item.quantity)
        sum_yen += parseFloat(item.yen.replace(/\./g, ""))
        sum_vnd += parseFloat(item.vnd.replace(/\./g, ""))
        tbCarts[i].sum_quantity = sum_quantity
        tbCarts[i].sum_yen = common.currency_format(sum_yen)
        tbCarts[i].sum_vnd = common.currency_format(sum_vnd)
        localStorage.setItem('tbCarts', JSON.stringify(tbCarts))
        if(item.checked == ''){
          sum_quantity -= parseFloat(item.quantity)
          sum_yen -= parseFloat(item.yen.replace(/\./g, ""))
          sum_vnd -= parseFloat(item.vnd.replace(/\./g, ""))
          tbCarts[i].sum_quantity = sum_quantity
          tbCarts[i].sum_yen = common.currency_format(sum_yen)
          tbCarts[i].sum_vnd = common.currency_format(sum_vnd)
          localStorage.setItem('tbCarts', JSON.stringify(tbCarts))
        }
      }
      if (checkboxes.length === checkboxes.filter(':checked').length) {
          checkall[0].checked = true
          shop.checkall = 'checked'
          localStorage.setItem('tbCarts', JSON.stringify(tbCarts))
      }
      else{
          checkall[0].checked = false
          shop.checkall = ''
          localStorage.setItem('tbCarts', JSON.stringify(tbCarts))
      }
      
      // sum_yen += shippingJ_fee_yen
      // sum_vnd += shippingJ_fee_vnd
      // tbCarts[i].sum_yen = common.currency_format(sum_yen)
      // tbCarts[i].sum_vnd = common.currency_format(sum_vnd)
      // localStorage.setItem('tbCarts', JSON.stringify(tbCarts))
      $(this).parents('.cart_info').find('.sum_quantity').html(sum_quantity)
      $(this).parents('.cart_info').find('.sum_yen').html(common.currency_format(sum_yen))
      $(this).parents('.cart_info').find('.sum_vnd').html(common.currency_format(sum_vnd))
    }
  }
  common.DisplaySumCart()
})

$(document).on('click', '#clearall', function () {
  var common = new CommonTool()
  var tbCarts = localStorage.setItem('tbCarts', null)
  tbCarts = []
  $('.wrapper').html('')
  common.DisplaySumCart()
})

$(document).on('click', '.cart_button_shop_delete', function () {
  var c = confirm('Bạn muốn xóa cửa hàng này?')
  if (c == true) {    
    var common = new CommonTool()
    var shop_id = $(this).parents('.addon_shop').find('.addon_shop_url').attr('data-key')
    tbCarts = localStorage.getItem('tbCarts')
    tbCarts = JSON.parse(tbCarts) // Converts string to object 
    if (tbCarts == null) // If there is no data, initialize an empty array 
      tbCarts = []
    for (var i = 0; i < tbCarts.length; i++) {
      var shop = tbCarts[i]
      if (shop.key == shop_id) {
        tbCarts.splice(i, 1);
      }
    }
    localStorage.setItem('tbCarts', JSON.stringify(tbCarts))
  }
  $(this).parents('.addon_shop').remove();
  common.DisplaySumCart()
})

$(document).on('click', '.item_delete', function () {
  var c = confirm('Bạn muốn xóa sản phẩm này?');
  if (c == true) {    
    var common = new CommonTool()
    var shop_id = $(this).parents('.cart_line').attr('data-key')
    var jan_code = $(this).parents('.cart_line').attr('data-jancode')
    var sum_quantity = 0
    var sum_yen = 0
    var sum_vnd = 0
    tbCarts = localStorage.getItem('tbCarts')
    tbCarts = JSON.parse(tbCarts) // Converts string to object 
    if (tbCarts == null) // If there is no data, initialize an empty array 
      tbCarts = []
      
    for (var i = 0; i < tbCarts.length; i++) {
      var shop = tbCarts[i]
      if (shop.key == shop_id) {
        var shopItem = shop.value
        sum_quantity += parseFloat(tbCarts[i].sum_quantity)
        sum_yen += parseFloat(tbCarts[i].sum_yen.replace(/\./g, ""))
        sum_vnd += parseFloat(tbCarts[i].sum_vnd.replace(/\./g, ""))
        for (var j = 0; j < shopItem.length; j++) {
          var item = shopItem[j]
          if (item.jan_code == jan_code) {
            sum_quantity -= parseFloat(item.quantity)
            sum_yen -= parseFloat(item.yen.replace(/\./g, ""))
            sum_vnd -= parseFloat(item.vnd.replace(/\./g, ""))
            tbCarts[i].sum_quantity = sum_quantity
            tbCarts[i].sum_yen = common.currency_format(sum_yen)
            tbCarts[i].sum_vnd = common.currency_format(sum_vnd)
            shopItem.splice(j, 1);
            localStorage.setItem('tbCarts', JSON.stringify(tbCarts))
          }          
        }

        if(shopItem.length < 1) {
          tbCarts.splice(i, 1);
          $(this).parents('.addon_shop').remove();
          localStorage.setItem('tbCarts', JSON.stringify(tbCarts))
        }
      }
      localStorage.setItem('tbCarts', JSON.stringify(tbCarts))
    }
  }
  $(this).parents('.cart_info').find('.sum_quantity').html(sum_quantity)
  $(this).parents('.cart_info').find('.sum_yen').html(common.currency_format(sum_yen))
  $(this).parents('.cart_info').find('.sum_vnd').html(common.currency_format(sum_vnd))
  $(this).parents('.cart_line').remove();
  common.DisplaySumCart()
})

$(document).on('click', '#export', function () {
  var ex_template = $('#ex_template').html()
  tbCarts = localStorage.getItem('tbCarts') // Retrieve the stored data 
  tbCarts = JSON.parse(tbCarts) // Converts string to object 
  if (tbCarts == null) // If there is no data, initialize an empty array 
    tbCarts = []
  var arrObj = _.sortByDesc(tbCarts, "time");
  for (var i = 0; i < arrObj.length; i++) {
    var item = arrObj[i];
    var ex_html = Mustache.to_html(ex_template, item).replace(/^\s*/mg, '')
    $('#export_template').append(ex_html)
  }
  
  $("#export_template").table2excel({
    exclude: "",
      name: "Thông tin giỏ hàng của bạn"
  }); 
});