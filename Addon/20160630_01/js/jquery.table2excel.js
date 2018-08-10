//table2excel.js
;(function ( $, window, document, undefined ) {
		var pluginName = "table2excel",
				defaults = {
				exclude: ".noExl",
                name: "Table2Excel",
		};

		// The actual plugin constructor
		function Plugin ( element, options ) {
				this.element = element;
				// jQuery has an extend method which merges the contents of two or
				// more objects, storing the result in the first object. The first object
				// is generally empty as we don't want to alter the default options for
				// future instances of the plugin
				this.settings = $.extend( {}, defaults, options );
				this._defaults = defaults;
				this._name = pluginName;
				this.init();
		}

		Plugin.prototype = {
			init: function () {
				var e = this;
				e.template = "<html xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:x=\"urn:schemas-microsoft-com:office:excel\" xmlns=\"http://www.w3.org/TR/REC-html40\"><head><!--[if gte mso 9]><xml>";
				e.template += "<x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions>";
				e.template += "<x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]-->";
				e.template += "</head><body><table x:str border=1 cellpadding=0 cellspacing=0 style=\"border-collapse: collapse\">{table}</table></body></html>";
				e.tableRows = "";

				// get contents of table except for exclude
				$(e.element).find("tr").not(this.settings.exclude).each(function (i,o) {
					e.tableRows += "<tr>" + $(o).html() + "</tr>";
				});
				// console.log(e.tableRows)
				this.tableToExcel(this.tableRows, this.settings.name);
			},
			tableToExcel: function (table, name) {
				var e = this;
				e.uri = "data:application/vnd.ms-excel;base64,";
				e.base64 = function (s) {
					return window.btoa(unescape(encodeURIComponent(s)));
				};
				e.format = function (s, c) {
					return s.replace(/{(\w+)}/g, function (m, p) {
						return c[p];
					});
				};
				e.ctx = {
					worksheet: name || "Worksheet",
					table: table,
				};
				// Name the downloaded file according to to the date
				var dt = new Date();
				var day = dt.getDate();
				var month = dt.getMonth() + 1;
				var year = dt.getFullYear();
				var hour = dt.getHours();
				var mins = dt.getMinutes();
				var random=Math.random();
				var postfix = day + "." + month + "." + year + "_" + hour + "." + mins;
				var a = document.createElement('a');
				a.href = e.uri + e.base64(e.format(e.template, e.ctx));
				a.download = 'Gio_hang_' + postfix+"_"+random + '.xls';
				a.click();
			}
		};

		$.fn[ pluginName ] = function ( options ) {
				this.each(function() {
						if ( !$.data( this, "plugin_" + pluginName ) ) {
								$.data( this, "plugin_" + pluginName, new Plugin( this, options ) );
						}
				});

				// chain jQuery functions
				return this;
		};

})( jQuery, window, document );