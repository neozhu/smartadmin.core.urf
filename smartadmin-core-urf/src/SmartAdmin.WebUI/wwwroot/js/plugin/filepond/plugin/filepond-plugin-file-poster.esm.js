/*!
 * FilePondPluginFilePoster 2.2.0
 * Licensed under MIT, https://opensource.org/licenses/MIT/
 * Please visit https://pqina.nl/filepond/ for details.
 */

/* eslint-disable */

const IMAGE_SCALE_SPRING_PROPS = {
  type: 'spring',
  stiffness: 0.5,
  damping: 0.45,
  mass: 10
};

const createPosterView = _ =>
  _.utils.createView({
    name: 'file-poster',
    tag: 'div',
    ignoreRect: true,
    create: ({ root }) => {
      root.ref.image = document.createElement('img');
      root.element.appendChild(root.ref.image);
    },
    write: _.utils.createRoute({
      DID_FILE_POSTER_LOAD: ({ root, props }) => {
        const { id } = props;

        // get item
        const item = root.query('GET_ITEM', { id: props.id });
        if (!item) return;

        // get poster
        const poster = item.getMetadata('poster');
        root.ref.image.src = poster;

        // let others know of our fabulous achievement (so the image can be faded in)
        root.dispatch('DID_FILE_POSTER_DRAW', { id });
      }
    }),
    mixins: {
      styles: ['scaleX', 'scaleY', 'opacity'],
      animations: {
        scaleX: IMAGE_SCALE_SPRING_PROPS,
        scaleY: IMAGE_SCALE_SPRING_PROPS,
        opacity: { type: 'tween', duration: 750 }
      }
    }
  });

const applyTemplate = (source, target) => {
  // copy width and height
  target.width = source.width;
  target.height = source.height;

  // draw the template
  const ctx = target.getContext('2d');
  ctx.drawImage(source, 0, 0);
};

const createPosterOverlayView = fpAPI =>
  fpAPI.utils.createView({
    name: 'file-poster-overlay',
    tag: 'canvas',
    ignoreRect: true,
    create: ({ root, props }) => {
      applyTemplate(props.template, root.element);
    },
    mixins: {
      styles: ['opacity'],
      animations: {
        opacity: { type: 'spring', mass: 25 }
      }
    }
  });

const getImageSize = (url, cb) => {
  let image = new Image();
  image.onload = () => {
    const width = image.naturalWidth;
    const height = image.naturalHeight;
    image = null;
    cb(width, height);
  };
  image.src = url;
};

const easeInOutSine = t => -0.5 * (Math.cos(Math.PI * t) - 1);

const addGradientSteps = (
  gradient,
  color,
  alpha = 1,
  easeFn = easeInOutSine,
  steps = 10,
  offset = 0
) => {
  const range = 1 - offset;
  const rgb = color.join(',');
  for (let i = 0; i <= steps; i++) {
    const p = i / steps;
    const stop = offset + range * p;
    gradient.addColorStop(stop, `rgba(${rgb}, ${easeFn(p) * alpha})`);
  }
};

const MAX_WIDTH = 10;
const MAX_HEIGHT = 10;

const calculateAverageColor = image => {
  const scalar = Math.min(MAX_WIDTH / image.width, MAX_HEIGHT / image.height);

  const canvas = document.createElement('canvas');
  const ctx = canvas.getContext('2d');
  const width = (canvas.width = Math.ceil(image.width * scalar));
  const height = (canvas.height = Math.ceil(image.height * scalar));
  ctx.drawImage(image, 0, 0, width, height);
  let data = null;
  try {
    data = ctx.getImageData(0, 0, width, height).data;
  } catch (e) {
    return null;
  }
  const l = data.length;

  let r = 0;
  let g = 0;
  let b = 0;
  let i = 0;

  for (; i < l; i += 4) {
    r += data[i] * data[i];
    g += data[i + 1] * data[i + 1];
    b += data[i + 2] * data[i + 2];
  }

  r = averageColor(r, l);
  g = averageColor(g, l);
  b = averageColor(b, l);

  return { r, g, b };
};

const averageColor = (c, l) => Math.floor(Math.sqrt(c / (l / 4)));

const drawTemplate = (canvas, width, height, color, alphaTarget) => {
  canvas.width = width;
  canvas.height = height;
  const ctx = canvas.getContext('2d');

  const horizontalCenter = width * 0.5;

  const grad = ctx.createRadialGradient(
    horizontalCenter,
    height + 110,
    height - 100,
    horizontalCenter,
    height + 110,
    height + 100
  );

  addGradientSteps(grad, color, alphaTarget, undefined, 8, 0.4);

  ctx.save();
  ctx.translate(-width * 0.5, 0);
  ctx.scale(2, 1);
  ctx.fillStyle = grad;
  ctx.fillRect(0, 0, width, height);
  ctx.restore();
};

const hasNavigator = typeof navigator !== 'undefined';

const width = 500;
const height = 200;

const overlayTemplateShadow = hasNavigator && document.createElement('canvas');
const overlayTemplateError = hasNavigator && document.createElement('canvas');
const overlayTemplateSuccess = hasNavigator && document.createElement('canvas');

if (hasNavigator) {
  drawTemplate(overlayTemplateShadow, width, height, [40, 40, 40], 0.85);
  drawTemplate(overlayTemplateError, width, height, [196, 78, 71], 1);
  drawTemplate(overlayTemplateSuccess, width, height, [54, 151, 99], 1);
}

const loadImage = (url, crossOriginValue) =>
  new Promise((resolve, reject) => {
    const img = new Image();
    if (typeof crossOrigin === 'string') {
      img.crossOrigin = crossOriginValue;
    }
    img.onload = () => {
      resolve(img);
    };
    img.onerror = e => {
      reject(e);
    };
    img.src = url;
  });

const createPosterWrapperView = _ => {
  // create overlay view
  const overlay = createPosterOverlayView(_);

  /**
   * Write handler for when preview container has been created
   */
  const didCreatePreviewContainer = ({ root, props }) => {
    const { id } = props;

    // we need to get the file data to determine the eventual image size
    const item = root.query('GET_ITEM', id);
    if (!item) return;

    // get url to file
    const fileURL = item.getMetadata('poster');

    // image is now ready
    const previewImageLoaded = data => {
      // calculate average image color, is in try catch to circumvent any cors errors
      const averageColor = root.query(
        'GET_FILE_POSTER_CALCULATE_AVERAGE_IMAGE_COLOR'
      )
        ? calculateAverageColor(data)
        : null;
      item.setMetadata('color', averageColor, true);

      // the preview is now ready to be drawn
      root.dispatch('DID_FILE_POSTER_LOAD', {
        id,
        data
      });
    };

    // determine image size of this item
    getImageSize(fileURL, (width, height) => {
      // we can now scale the panel to the final size
      root.dispatch('DID_FILE_POSTER_CALCULATE_SIZE', {
        id,
        width,
        height
      });

      // create fallback preview
      loadImage(
        fileURL,
        root.query('GET_FILE_POSTER_CROSS_ORIGIN_ATTRIBUTE_VALUE')
      ).then(previewImageLoaded);
    });
  };

  /**
   * Write handler for when the preview has been loaded
   */
  const didLoadPreview = ({ root }) => {
    root.ref.overlayShadow.opacity = 1;
  };

  /**
   * Write handler for when the preview image is ready to be animated
   */
  const didDrawPreview = ({ root }) => {
    const { image } = root.ref;

    // reveal image
    image.scaleX = 1.0;
    image.scaleY = 1.0;
    image.opacity = 1;
  };

  /**
   * Write handler for when the preview has been loaded
   */
  const restoreOverlay = ({ root }) => {
    root.ref.overlayShadow.opacity = 1;
    root.ref.overlayError.opacity = 0;
    root.ref.overlaySuccess.opacity = 0;
  };

  const didThrowError = ({ root }) => {
    root.ref.overlayShadow.opacity = 0.25;
    root.ref.overlayError.opacity = 1;
  };

  const didCompleteProcessing = ({ root }) => {
    root.ref.overlayShadow.opacity = 0.25;
    root.ref.overlaySuccess.opacity = 1;
  };

  /**
   * Constructor
   */
  const create = ({ root, props }) => {
    // image view
    const image = createPosterView(_);

    // append image presenter
    root.ref.image = root.appendChildView(
      root.createChildView(image, {
        id: props.id,
        scaleX: 1.25,
        scaleY: 1.25,
        opacity: 0
      })
    );

    // image overlays
    root.ref.overlayShadow = root.appendChildView(
      root.createChildView(overlay, {
        template: overlayTemplateShadow,
        opacity: 0
      })
    );

    root.ref.overlaySuccess = root.appendChildView(
      root.createChildView(overlay, {
        template: overlayTemplateSuccess,
        opacity: 0
      })
    );

    root.ref.overlayError = root.appendChildView(
      root.createChildView(overlay, {
        template: overlayTemplateError,
        opacity: 0
      })
    );
  };

  return _.utils.createView({
    name: 'file-poster-wrapper',
    create,
    write: _.utils.createRoute({
      // image preview stated
      DID_FILE_POSTER_LOAD: didLoadPreview,
      DID_FILE_POSTER_DRAW: didDrawPreview,
      DID_FILE_POSTER_CONTAINER_CREATE: didCreatePreviewContainer,

      // file states
      DID_THROW_ITEM_LOAD_ERROR: didThrowError,
      DID_THROW_ITEM_PROCESSING_ERROR: didThrowError,
      DID_THROW_ITEM_INVALID: didThrowError,
      DID_COMPLETE_ITEM_PROCESSING: didCompleteProcessing,
      DID_START_ITEM_PROCESSING: restoreOverlay,
      DID_REVERT_ITEM_PROCESSING: restoreOverlay
    })
  });
};

/**
 * File Poster Plugin
 */
const plugin = fpAPI => {
  const { addFilter, utils } = fpAPI;
  const { Type, createRoute } = utils;

  // filePosterView
  const filePosterView = createPosterWrapperView(fpAPI);

  // called for each view that is created right after the 'create' method
  addFilter('CREATE_VIEW', viewAPI => {
    // get reference to created view
    const { is, view, query } = viewAPI;

    // only hook up to item view and only if is enabled for this cropper
    if (!is('file') || !query('GET_ALLOW_FILE_POSTER')) return;

    // create the file poster plugin, but only do so if the item is an image
    const didLoadItem = ({ root, props }) => {
      const { id } = props;
      const item = query('GET_ITEM', id);

      // item could theoretically have been removed in the mean time
      if (!item || !item.getMetadata('poster') || item.archived) return;

      // test if is filtered
      if (!query('GET_FILE_POSTER_FILTER_ITEM')(item)) return;

      // set preview view
      root.ref.filePoster = view.appendChildView(
        view.createChildView(filePosterView, { id })
      );

      // now ready
      root.dispatch('DID_FILE_POSTER_CONTAINER_CREATE', { id });
    };

    const didCalculatePreviewSize = ({ root, action }) => {
      // no poster set
      if (!root.ref.filePoster) return;

      // remember dimensions
      root.ref.imageWidth = action.width;
      root.ref.imageHeight = action.height;

      root.ref.shouldUpdatePanelHeight = true;

      root.dispatch('KICK');
    };

    // start writing
    view.registerWriter(
      createRoute(
        {
          DID_LOAD_ITEM: didLoadItem,
          DID_FILE_POSTER_CALCULATE_SIZE: didCalculatePreviewSize
        },
        ({ root, props }) => {
          // don't run without poster
          if (!root.ref.filePoster) return;

          // don't do anything while hidden
          if (root.rect.element.hidden) return;

          // should we redraw
          if (root.ref.shouldUpdatePanelHeight) {
            // time to resize the parent panel
            root.dispatch('DID_UPDATE_PANEL_HEIGHT', {
              id: props.id,
              height:
                root.rect.element.width *
                (root.ref.imageHeight / root.ref.imageWidth)
            });

            // done!
            root.ref.shouldUpdatePanelHeight = false;
          }
        }
      )
    );
  });

  // expose plugin
  return {
    options: {
      // Enable or disable file poster
      allowFilePoster: [true, Type.BOOLEAN],

      // filters file items to determine which are shown as poster
      filePosterFilterItem: [() => true, Type.FUNCTION],

      // Enables or disables reading average image color
      filePosterCalculateAverageImageColor: [false, Type.BOOLEAN],

      // Allows setting the value of the CORS attribute (null is don't set attribute)
      filePosterCrossOriginAttributeValue: ['Anonymous', Type.STRING]
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
