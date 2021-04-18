/*!
 * FilePondPluginFileRename 1.1.8
 * Licensed under MIT, https://opensource.org/licenses/MIT/
 * Please visit https://pqina.nl/filepond/ for details.
 */

/* eslint-disable */

const plugin = ({ addFilter, utils }) => {
  // get quick reference to Type utils
  const {
    Type,
    renameFile,
    isFile,
    getExtensionFromFilename,
    getFilenameWithoutExtension
  } = utils;

  // called for each file that is loaded
  // right before it is set to the item state
  // should return a promise
  addFilter(
    'LOAD_FILE',
    (file, { query }) =>
      new Promise((resolve, reject) => {
        // reject
        const allowFileRename = query('GET_ALLOW_FILE_RENAME');
        const renameFunction = query('GET_FILE_RENAME_FUNCTION');
        if (!isFile(file) || !allowFileRename || !renameFunction) {
          resolve(file);
          return;
        }

        // can either return a name or a promise
        const newFilename = renameFunction({
          name: file.name,
          basename: getFilenameWithoutExtension(file.name),
          extension: `.${getExtensionFromFilename(file.name)}`
        });

        // renames the file and resolves
        const rename = name => {
          resolve(renameFile(file, name));
        };

        // has returned new filename immidiately
        if (typeof newFilename === 'string') {
          rename(newFilename);
          return;
        }

        // is promise
        newFilename.then(rename);
      })
  );

  return {
    options: {
      // Enable or disable file renaming
      allowFileRename: [true, Type.BOOLEAN],

      // Rename function to run for this
      fileRenameFunction: [null, Type.FUNCTION]
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
