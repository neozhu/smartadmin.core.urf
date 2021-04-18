/*!
 * FilePondPluginFileEncode 2.1.10
 * Licensed under MIT, https://opensource.org/licenses/MIT/
 * Please visit https://pqina.nl/filepond/ for details.
 */

/* eslint-disable */

const DataURIWorker = function() {
  // route messages
  self.onmessage = e => {
    convert(e.data.message, response => {
      self.postMessage({ id: e.data.id, message: response });
    });
  };

  // convert file to data uri
  const convert = (options, cb) => {
    const { file } = options;

    const reader = new FileReader();
    reader.onloadend = () => {
      cb(reader.result.replace('data:', '').replace(/^.+,/, ''));
    };
    reader.readAsDataURL(file);
  };
};

const plugin = ({ addFilter, utils }) => {
  // get quick reference to Type utils
  const { Type, createWorker, createRoute, isFile } = utils;

  const encode = ({ name, file }) =>
    new Promise(resolve => {
      const worker = createWorker(DataURIWorker);
      worker.post({ file }, data => {
        resolve({ name, data });
        worker.terminate();
      });
    });

  // holds base64 strings till can be moved to item
  const base64Cache = [];
  addFilter('DID_CREATE_ITEM', (item, { query }) => {
    if (!query('GET_ALLOW_FILE_ENCODE')) return;

    item.extend(
      'getFileEncodeBase64String',
      () => base64Cache[item.id] && base64Cache[item.id].data
    );
    item.extend(
      'getFileEncodeDataURL',
      () => `data:${item.fileType};base64,${base64Cache[item.id].data}`
    );
  });

  addFilter(
    'SHOULD_PREPARE_OUTPUT',
    (shouldPrepareOutput, { query }) =>
      new Promise(resolve => {
        resolve(query('GET_ALLOW_FILE_ENCODE'));
      })
  );

  addFilter(
    'COMPLETE_PREPARE_OUTPUT',
    (file, { item, query }) =>
      new Promise(resolve => {
        // if it's not a file or a list of files, continue
        if (
          !query('GET_ALLOW_FILE_ENCODE') ||
          (!isFile(file) && !Array.isArray(file))
        ) {
          return resolve(file);
        }

        // store metadata settings for this cache
        base64Cache[item.id] = {
          metadata: item.getMetadata(),
          data: null
        };

        // wait for all file items to be encoded
        Promise.all(
          (file instanceof Blob ? [{ name: null, file }] : file).map(encode)
        ).then(dataItems => {
          base64Cache[item.id].data =
            file instanceof Blob ? dataItems[0].data : dataItems;
          resolve(file);
        });
      })
  );

  // called for each view that is created right after the 'create' method
  addFilter('CREATE_VIEW', viewAPI => {
    // get reference to created view
    const { is, view, query } = viewAPI;

    // only hook up to item view
    if (!is('file-wrapper') || !query('GET_ALLOW_FILE_ENCODE')) {
      return;
    }

    view.registerWriter(
      createRoute({
        DID_PREPARE_OUTPUT: ({ root, action }) => {
          // only do this if is not uploading async
          if (query('IS_ASYNC')) {
            return;
          }

          const item = query('GET_ITEM', action.id);
          if (!item) return;

          // extract base64 string
          const cache = base64Cache[item.id];
          const metadata = cache.metadata;
          const data = cache.data;

          // create JSON string from encoded data
          const value = JSON.stringify({
            id: item.id,
            name: item.file.name,
            type: item.file.type,
            size: item.file.size,
            metadata: metadata,
            data
          });

          // for filepond < 4.13.0
          if (root.ref.data) {
            root.ref.data.value = value;
          }
          // newer versions
          else {
            root.dispatch('DID_DEFINE_VALUE', {
              id: item.id,
              value
            });
          }
        },
        DID_REMOVE_ITEM: ({ action }) => {
          const item = query('GET_ITEM', action.id);
          if (!item) return;
          delete base64Cache[item.id];
        }
      })
    );
  });

  return {
    options: {
      // Enable or disable file encoding
      allowFileEncode: [true, Type.BOOLEAN]
    }
  };
};

// fire pluginloaded event if running in browser, this allows registering the plugin when using async script tags
const isBrowser =
  typeof window !== 'undefined' && typeof window.document !== 'undefined';
if (isBrowser) {
  document.dispatchEvent(
    new CustomEvent('FilePond:pluginloaded', { detail: plugin })
  );
}

export default plugin;
