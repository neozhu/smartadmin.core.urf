$(document).ready(function () {

  // enable fileuploader plugin
  $('input[name="files"]').fileuploader({
    limit: 1,
    maxSize: 20,
    fileMaxSize: 20,
    captions: {
      feedback: 'Drag and drop files here',
      feedback2: 'Drag and drop files here',
      drop: 'Drag and drop files here',
      or: 'or',
      button: 'Browse files'
    },
    changeInput: '<div class="fileuploader-input">' +
      '<div class="fileuploader-input-inner">' +
      '<div class="fileuploader-main-icon"></div>' +
      '<h3 class="fileuploader-input-caption"><span>Drag and drop files here</span></h3>' +
      '<p>or</p>' +
      '<div class="fileuploader-input-button"><span>Browse files</span></div>' +
      '</div>' +
      '</div>',
    theme: 'dragdrop',
    extensions: ['xlsx', 'csv', 'text/plain'],
    upload: {
      url: "/FileUpload/Upload",
      data: { FileType: entityname },
      type: 'POST',
      enctype: 'multipart/form-data',
      start: true,
      synchron: false,
      beforeSend: function (item) {
        item.upload.data.ver = '2.0';
        item.upload.data.data_attribute = 'nice';
      },
      onSuccess: function (result, item) {
        // if success
        if (result.success && result.filename) {
          //item.name = result.filename;
          item.html.find('.column-title > div:first-child').text(item.name).attr('title', result.filename);
          $.messager.alert('导入完成', '导入完成！<br> 耗时：' + result.elapsedTime, 'info');
          $dg.datagrid('reload');
          $('#importwindow').window('close');
        } else {
          item.html.find('.column-title > div:first-child').text(item.name).attr('title', result.filename);
          item.html.removeClass('upload-successful').addClass('upload-failed');
          // go out from success function by calling onError function
          // in this case we have a animation there
          // you can also response in PHP with 404
          //alert(result.message);
          $.messager.alert('导入失败', result.message, 'error');
          return this.onError ? this.onError(item) : null;

        }



        item.html.find('.fileuploader-action-remove').addClass('fileuploader-action-success');
        setTimeout(function () {
          item.html.find('.progress-bar2').fadeOut(400);
        }, 400);
      },
      onError: function (item) {
        var progressBar = item.html.find('.progress-bar2');

        if (progressBar.length) {
          progressBar.find('span').html(0 + "%");
          progressBar.find('.fileuploader-progressbar .bar').width(0 + "%");
          item.html.find('.progress-bar2').fadeOut(400);
        }

        item.upload.status != 'cancelled' && item.html.find('.fileuploader-action-retry').length == 0 ? item.html.find('.column-actions').prepend(
          '<a class="fileuploader-action fileuploader-action-retry" title="Retry"><i></i></a>'
        ) : null;
      },
      onProgress: function (data, item) {
        var progressBar = item.html.find('.progress-bar2');

        if (progressBar.length > 0) {
          progressBar.show();
          progressBar.find('span').html(data.percentage + "%");
          progressBar.find('.fileuploader-progressbar .bar').width(data.percentage + "%");
        }
      },
      onComplete: null
    },
    onRemove: function (item) {

      var filename = item.html.find('.column-title > div:first-child').attr('title');
      $.post('/FileUpload/Remove', { filename: filename });

    },
    thumbnails: {
      canvasImage: false,
      popup: null
    }
  });

});
