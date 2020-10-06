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
      var xhr = new XMLHttpRequest();
      xhr.open('POST', url, true);
      xhr.responseType = 'arraybuffer';
      xhr.send(formData);
      xhr.overrideMimeType("application/vnd.ms-excel");
      xhr.onreadystatechange = function () {
        if (xhr.status === 500) {
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
      xhr.onload = function () {
        if (this.status === 200) {
          var filename = "";
          var disposition = xhr.getResponseHeader('Content-Disposition');
          if (disposition && disposition.indexOf('attachment') !== -1) {
            var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
            var matches = filenameRegex.exec(disposition);
            if (matches != null && matches[1]) filename = matches[1].replace(/['"]/g, '');
          }
          var type = xhr.getResponseHeader('Content-Type');

          var blob;
          if (typeof File === 'function') {
            try {
              blob = new File([this.response], filename, { type: type });
            } catch (e) { /* Edge */ }
          }
          if (typeof blob === 'undefined') {
            blob = new Blob([this.response], { type: type });
          }

          if (typeof window.navigator.msSaveBlob !== 'undefined') {
            // IE workaround for "HTML7007: One or more blob URLs were revoked by closing the blob for which they were created. These URLs will no longer resolve as the data backing the URL has been freed."
            window.navigator.msSaveBlob(blob, filename);
          } else {
            var URL = window.URL || window.webkitURL;
            var downloadUrl = URL.createObjectURL(blob);

            if (filename) {
              // use HTML5 a[download] attribute to specify filename
              var a = document.createElement("a");
              // safari doesn't support this yet
              if (typeof a.download === 'undefined') {
                window.location.href = downloadUrl;
              } else {
                a.href = downloadUrl;
                a.download = filename;
                document.body.appendChild(a);
                a.click();
              }
            } else {
              window.location.href = downloadUrl;
            }

            setTimeout(function () { URL.revokeObjectURL(downloadUrl); }, 100); // cleanup


          }
          resolve({
            filename: filename,
            status: this.status,
            statusText: xhr.statusText
          });
        }
        else {
          reject({
            status: this.status,
            statusText: this.statusText
          });
        }
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
