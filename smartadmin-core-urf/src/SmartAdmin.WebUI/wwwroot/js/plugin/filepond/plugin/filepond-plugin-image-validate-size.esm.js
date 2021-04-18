/*!
 * FilePondPluginImageValidateSize 1.2.6
 * Licensed under MIT, https://opensource.org/licenses/MIT/
 * Please visit https://pqina.nl/filepond/ for details.
 */

/* eslint-disable */

// test if file is of type image
const isImage = file => /^image/.test(file.type);

const getImageSize = file =>
  new Promise((resolve, reject) => {
    const image = document.createElement('img');
    image.src = URL.createObjectURL(file);
    image.onerror = err => {
      clearInterval(intervalId);
      reject(err);
    };
    const intervalId = setInterval(() => {
      if (image.naturalWidth && image.naturalHeight) {
        clearInterval(intervalId);
        URL.revokeObjectURL(image.src);
        resolve({
          width: image.naturalWidth,
          height: image.naturalHeight
        });
      }
    }, 1);
  });

const plugin = ({ addFilter, utils }) => {
  // get quick reference to Type utils
  const { Type, replaceInString, isFile } = utils;

  // required file size
  const validateFile = (file, bounds, measure) =>
    new Promise((resolve, reject) => {
      const onReceiveSize = ({ width, height }) => {
        const {
          minWidth,
          minHeight,
          maxWidth,
          maxHeight,
          minResolution,
          maxResolution
        } = bounds;

        const resolution = width * height;

        // validation result
        if (width < minWidth || height < minHeight) {
          reject('TOO_SMALL');
        } else if (width > maxWidth || height > maxHeight) {
          reject('TOO_BIG');
        } else if (minResolution !== null && resolution < minResolution) {
          reject('TOO_LOW_RES');
        } else if (maxResolution !== null && resolution > maxResolution) {
          reject('TOO_HIGH_RES');
        }

        // all is well
        resolve();
      };

      getImageSize(file)
        .then(onReceiveSize)
        .catch(() => {
          // no custom measure method supplied, exit here
          if (!measure) {
            reject();
            return;
          }

          // try fallback if defined by user, else reject
          measure(file, bounds)
            .then(onReceiveSize)
            .catch(() => reject());
        });
    });

  // called for each file that is loaded
  // right before it is set to the item state
  // should return a promise
  addFilter(
    'LOAD_FILE',
    (file, { query }) =>
      new Promise((resolve, reject) => {
        if (
          !isFile(file) ||
          !isImage(file) ||
          !query('GET_ALLOW_IMAGE_VALIDATE_SIZE')
        ) {
          resolve(file);
          return;
        }

        // get required dimensions
        const bounds = {
          minWidth: query('GET_IMAGE_VALIDATE_SIZE_MIN_WIDTH'),
          minHeight: query('GET_IMAGE_VALIDATE_SIZE_MIN_HEIGHT'),
          maxWidth: query('GET_IMAGE_VALIDATE_SIZE_MAX_WIDTH'),
          maxHeight: query('GET_IMAGE_VALIDATE_SIZE_MAX_HEIGHT'),
          minResolution: query('GET_IMAGE_VALIDATE_SIZE_MIN_RESOLUTION'),
          maxResolution: query('GET_IMAGE_VALIDATE_SIZE_MAX_RESOLUTION')
        };

        // get optional custom measure function
        const measure = query('GET_IMAGE_VALIDATE_SIZE_MEASURE');

        validateFile(file, bounds, measure)
          .then(() => {
            resolve(file);
          })
          .catch(error => {
            const status = error
              ? {
                  TOO_SMALL: {
                    label: query(
                      'GET_IMAGE_VALIDATE_SIZE_LABEL_IMAGE_SIZE_TOO_SMALL'
                    ),
                    details: query(
                      'GET_IMAGE_VALIDATE_SIZE_LABEL_EXPECTED_MIN_SIZE'
                    )
                  },
                  TOO_BIG: {
                    label: query(
                      'GET_IMAGE_VALIDATE_SIZE_LABEL_IMAGE_SIZE_TOO_BIG'
                    ),
                    details: query(
                      'GET_IMAGE_VALIDATE_SIZE_LABEL_EXPECTED_MAX_SIZE'
                    )
                  },
                  TOO_LOW_RES: {
                    label: query(
                      'GET_IMAGE_VALIDATE_SIZE_LABEL_IMAGE_RESOLUTION_TOO_LOW'
                    ),
                    details: query(
                      'GET_IMAGE_VALIDATE_SIZE_LABEL_EXPECTED_MIN_RESOLUTION'
                    )
                  },
                  TOO_HIGH_RES: {
                    label: query(
                      'GET_IMAGE_VALIDATE_SIZE_LABEL_IMAGE_RESOLUTION_TOO_HIGH'
                    ),
                    details: query(
                      'GET_IMAGE_VALIDATE_SIZE_LABEL_EXPECTED_MAX_RESOLUTION'
                    )
                  }
                }[error]
              : {
                  label: query('GET_IMAGE_VALIDATE_SIZE_LABEL_FORMAT_ERROR'),
                  details: file.type
                };

            reject({
              status: {
                main: status.label,
                sub: error
                  ? replaceInString(status.details, bounds)
                  : status.details
              }
            });
          });
      })
  );

  // expose plugin
  return {
    // default options
    options: {
      // Enable or disable file type validation
      allowImageValidateSize: [true, Type.BOOLEAN],

      // Error thrown when image can not be loaded
      imageValidateSizeLabelFormatError: [
        'Image type not supported',
        Type.STRING
      ],

      // Custom function to use as image measure
      imageValidateSizeMeasure: [null, Type.FUNCTION],

      // Required amount of pixels in the image
      imageValidateSizeMinResolution: [null, Type.INT],
      imageValidateSizeMaxResolution: [null, Type.INT],
      imageValidateSizeLabelImageResolutionTooLow: [
        'Resolution is too low',
        Type.STRING
      ],
      imageValidateSizeLabelImageResolutionTooHigh: [
        'Resolution is too high',
        Type.STRING
      ],
      imageValidateSizeLabelExpectedMinResolution: [
        'Minimum resolution is {minResolution}',
        Type.STRING
      ],
      imageValidateSizeLabelExpectedMaxResolution: [
        'Maximum resolution is {maxResolution}',
        Type.STRING
      ],

      // Required dimensions
      imageValidateSizeMinWidth: [1, Type.INT], // needs to be at least one pixel
      imageValidateSizeMinHeight: [1, Type.INT],
      imageValidateSizeMaxWidth: [65535, Type.INT], // maximum size of JPEG, fine for now I guess
      imageValidateSizeMaxHeight: [65535, Type.INT],

      // Label to show when an image is too small or image is too big
      imageValidateSizeLabelImageSizeTooSmall: [
        'Image is too small',
        Type.STRING
      ],
      imageValidateSizeLabelImageSizeTooBig: ['Image is too big', Type.STRING],
      imageValidateSizeLabelExpectedMinSize: [
        'Minimum size is {minWidth} × {minHeight}',
        Type.STRING
      ],
      imageValidateSizeLabelExpectedMaxSize: [
        'Maximum size is {maxWidth} × {maxHeight}',
        Type.STRING
      ]
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
