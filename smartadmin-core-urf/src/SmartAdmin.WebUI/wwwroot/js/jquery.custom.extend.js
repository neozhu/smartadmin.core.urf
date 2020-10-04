moment.suppressDeprecationWarnings = true;
jQuery.extend({
  date: function () {
    //console.log(new Date());
    return moment(new Date()).format('YYYY-MM-DD');
  },
  datetime: function () {
    //console.log(new Date());
    return moment(new Date()).format('YYYY-MM-DD HH:mm:ss');
  },
  isDate: function (value) {

    var regex = '^\d{4}-\d{2}-\d{2}';
    return value.match(regex);
  },
  isDatetime: function (value) {

    var regex = new RegExp("/(\d{4})-(\d{2})-(\d{2}) (\d{2}):(\d{2}):(\d{2})/");
    return regex.test(value);
  },
  getuser: function () {
    return $('#username').val();
  },
  getusername: function () {
    return $('#user_fullname').html();
  },
  uuid: function () {
    var d = new Date().getTime();
    var guid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
      var r = (d + Math.random() * 16) % 16 | 0;
      d = Math.floor(d / 16);
      return (c === 'x' ? r : (r & 0x7 | 0x8)).toString(16);
    });
    return guid;
  },
  istrue: function (value) {
    if (typeof (value) === 'string') {
      value = value.trim().toLowerCase();
    }
    switch (true) {
      case true === value:
      case "true" === value:
      case value > 0:
      case "ok" === value:
      case "yes" === value:
      case "是" === value:
        return true;
      case "on" === value:
      case false === value:
      case "cancel" === value:
      case "null" === value:
      case "" === value:
      case "否" === value:
      case value <= 0:
        return false;
      default:
        return false;
    }
  },
  postDownload: function (url, formData, onCompleted) {
    return new Promise(function (resolve, reject) {
      var filename = '';
      var xhr = new XMLHttpRequest();
      xhr.open('POST', url, true);
      xhr.responseType = 'blob';
      xhr.send(formData);
      xhr.overrideMimeType("application/vnd.ms-excel");
      xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
          //console.log(xhr.getResponseHeader('Content-Disposition'));
          var header = xhr.getResponseHeader('Content-Disposition');
          if (header) {
            var regx = /filename[^;=\n]*=((['\"]).*?\2|[^;\n]*)/g;
            filename = regx.exec(header)[1];
            //console.log(regx.exec(header));
          }
          var blob = xhr.response;
          saveAs(blob, filename);
          if (onCompleted !== undefined)
            onCompleted(filename);

        } else if (xhr.status === 500) {
          reject({
            status: this.status,
            statusText: this.statusText
          });
        }
      };
      xhr.onerror = function () {
        reject({
          status: this.status,
          statusText: this.statusText
        });
      };
      xhr.onload = function (e) {
        resolve({
          filename: filename,
          status: this.status,
          statusText: xhr.statusText
        });
      };
     
    });
  }
});

(function ($) {
  $.fn.fromJSON = function (data) {
    var $form = $(this)
    //var data = jsonobj;
    $.each(data, function (key, value) {
      var $elem = $('[name="' + key + '"]', $form)
      var type = $elem.first().attr('type')
      if (type == 'radio') {
        $('[name="' + key + '"][value="' + value + '"]').prop('checked', true)
      } else if (type == 'checkbox' && (value == true || value == 'true')) {
        $('[name="' + key + '"]').prop('checked', true)
      } else {
        $elem.val(value)
      }
    })
  };
  $.fn.clearForm = function (options) {

    // This is the easiest way to have default options.
    var settings = $.extend({
      // These are the defaults.

      formId: this.closest('form')

    }, options);

    var $form = $(settings.formId);

    //reset jQuery Validate's internals
    $form.validate().resetForm();
    //reset unobtrusive validation summary, if it exists
    $form.find("[data-valmsg-summary=true]")
      .removeClass("validation-summary-errors")
      .addClass("validation-summary-valid")
      .find("ul").empty();

    //reset unobtrusive field level, if it exists
    $form.find("[data-valmsg-replace]")
      .removeClass("field-validation-error")
      .addClass("field-validation-valid")
      .empty();
    //reset unobtrusive field vaild/invalid status
    $form.find("[data-val=true]")
      .removeClass("is-valid")
      .removeClass("is-invalid")

    return $form;
  }
}(jQuery));
