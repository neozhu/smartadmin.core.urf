/*!
 * FilePondPluginImageTransform 3.5.2
 * Licensed under MIT, https://opensource.org/licenses/MIT/
 * Please visit https://pqina.nl/filepond/ for details.
 */

/* eslint-disable */

// test if file is of type image
const isImage = file => /^image/.test(file.type);

const getFilenameWithoutExtension = name =>
  name.substr(0, name.lastIndexOf('.')) || name;

// only handles image/jpg, image/jpeg, image/png, and image/svg+xml for now
const ExtensionMap = {
  jpeg: 'jpg',
  'svg+xml': 'svg'
};

const renameFileToMatchMimeType = (filename, mimeType) => {
  const name = getFilenameWithoutExtension(filename);
  const type = mimeType.split('/')[1];
  const extension = ExtensionMap[type] || type;
  return `${name}.${extension}`;
};

// returns all the valid output formats we can encode towards
const getValidOutputMimeType = type =>
  /jpeg|png|svg\+xml/.test(type) ? type : 'image/jpeg';

// test if file is of type image
const isImage$1 = file => /^image/.test(file.type);

const MATRICES = {
  1: () => [1, 0, 0, 1, 0, 0],
  2: width => [-1, 0, 0, 1, width, 0],
  3: (width, height) => [-1, 0, 0, -1, width, height],
  4: (width, height) => [1, 0, 0, -1, 0, height],
  5: () => [0, 1, 1, 0, 0, 0],
  6: (width, height) => [0, 1, -1, 0, height, 0],
  7: (width, height) => [0, -1, -1, 0, height, width],
  8: width => [0, -1, 1, 0, 0, width]
};

const getImageOrientationMatrix = (width, height, orientation) => {
  if (orientation === -1) {
    orientation = 1;
  }
  return MATRICES[orientation](width, height);
};

const createVector = (x, y) => ({ x, y });

const vectorDot = (a, b) => a.x * b.x + a.y * b.y;

const vectorSubtract = (a, b) => createVector(a.x - b.x, a.y - b.y);

const vectorDistanceSquared = (a, b) =>
  vectorDot(vectorSubtract(a, b), vectorSubtract(a, b));

const vectorDistance = (a, b) => Math.sqrt(vectorDistanceSquared(a, b));

const getOffsetPointOnEdge = (length, rotation) => {
  const a = length;

  const A = 1.5707963267948966;
  const B = rotation;
  const C = 1.5707963267948966 - rotation;

  const sinA = Math.sin(A);
  const sinB = Math.sin(B);
  const sinC = Math.sin(C);
  const cosC = Math.cos(C);
  const ratio = a / sinA;
  const b = ratio * sinB;
  const c = ratio * sinC;

  return createVector(cosC * b, cosC * c);
};

const getRotatedRectSize = (rect, rotation) => {
  const w = rect.width;
  const h = rect.height;

  const hor = getOffsetPointOnEdge(w, rotation);
  const ver = getOffsetPointOnEdge(h, rotation);

  const tl = createVector(rect.x + Math.abs(hor.x), rect.y - Math.abs(hor.y));

  const tr = createVector(
    rect.x + rect.width + Math.abs(ver.y),
    rect.y + Math.abs(ver.x)
  );

  const bl = createVector(
    rect.x - Math.abs(ver.y),
    rect.y + rect.height - Math.abs(ver.x)
  );

  return {
    width: vectorDistance(tl, tr),
    height: vectorDistance(tl, bl)
  };
};

const getImageRectZoomFactor = (
  imageRect,
  cropRect,
  rotation = 0,
  center = { x: 0.5, y: 0.5 }
) => {
  // calculate available space round image center position
  const cx = center.x > 0.5 ? 1 - center.x : center.x;
  const cy = center.y > 0.5 ? 1 - center.y : center.y;
  const imageWidth = cx * 2 * imageRect.width;
  const imageHeight = cy * 2 * imageRect.height;

  // calculate rotated crop rectangle size
  const rotatedCropSize = getRotatedRectSize(cropRect, rotation);

  return Math.max(
    rotatedCropSize.width / imageWidth,
    rotatedCropSize.height / imageHeight
  );
};

const getCenteredCropRect = (container, aspectRatio) => {
  let width = container.width;
  let height = width * aspectRatio;
  if (height > container.height) {
    height = container.height;
    width = height / aspectRatio;
  }
  const x = (container.width - width) * 0.5;
  const y = (container.height - height) * 0.5;

  return {
    x,
    y,
    width,
    height
  };
};

const calculateCanvasSize = (image, canvasAspectRatio, zoom = 1) => {
  const imageAspectRatio = image.height / image.width;

  // determine actual pixels on x and y axis
  let canvasWidth = 1;
  let canvasHeight = canvasAspectRatio;
  let imgWidth = 1;
  let imgHeight = imageAspectRatio;
  if (imgHeight > canvasHeight) {
    imgHeight = canvasHeight;
    imgWidth = imgHeight / imageAspectRatio;
  }

  const scalar = Math.max(canvasWidth / imgWidth, canvasHeight / imgHeight);
  const width = image.width / (zoom * scalar * imgWidth);
  const height = width * canvasAspectRatio;

  return {
    width: width,
    height: height
  };
};

const canvasRelease = canvas => {
  canvas.width = 1;
  canvas.height = 1;
  const ctx = canvas.getContext('2d');
  ctx.clearRect(0, 0, 1, 1);
};

const isFlipped = flip => flip && (flip.horizontal || flip.vertical);

const getBitmap = (image, orientation, flip) => {
  if (orientation <= 1 && !isFlipped(flip)) {
    image.width = image.naturalWidth;
    image.height = image.naturalHeight;
    return image;
  }

  const canvas = document.createElement('canvas');
  const width = image.naturalWidth;
  const height = image.naturalHeight;

  // if is rotated incorrectly swap width and height
  const swapped = orientation >= 5 && orientation <= 8;
  if (swapped) {
    canvas.width = height;
    canvas.height = width;
  } else {
    canvas.width = width;
    canvas.height = height;
  }

  // draw the image but first fix orientation and set correct flip
  const ctx = canvas.getContext('2d');

  // get base transformation matrix
  if (orientation) {
    ctx.transform.apply(
      ctx,
      getImageOrientationMatrix(width, height, orientation)
    );
  }

  if (isFlipped(flip)) {
    // flip horizontal
    // [-1, 0, 0, 1, width, 0]
    const matrix = [1, 0, 0, 1, 0, 0];
    if ((!swapped && flip.horizontal) || swapped & flip.vertical) {
      matrix[0] = -1;
      matrix[4] = width;
    }

    // flip vertical
    // [1, 0, 0, -1, 0, height]
    if ((!swapped && flip.vertical) || (swapped && flip.horizontal)) {
      matrix[3] = -1;
      matrix[5] = height;
    }

    ctx.transform(...matrix);
  }

  ctx.drawImage(image, 0, 0, width, height);

  return canvas;
};

const imageToImageData = (
  imageElement,
  orientation,
  crop = {},
  options = {}
) => {
  const { canvasMemoryLimit, background = null } = options;

  const zoom = crop.zoom || 1;

  // fixes possible image orientation problems by drawing the image on the correct canvas
  const bitmap = getBitmap(imageElement, orientation, crop.flip);
  const imageSize = {
    width: bitmap.width,
    height: bitmap.height
  };

  const aspectRatio = crop.aspectRatio || imageSize.height / imageSize.width;

  let canvasSize = calculateCanvasSize(imageSize, aspectRatio, zoom);

  if (canvasMemoryLimit) {
    const requiredMemory = canvasSize.width * canvasSize.height;
    if (requiredMemory > canvasMemoryLimit) {
      const scalar = Math.sqrt(canvasMemoryLimit) / Math.sqrt(requiredMemory);
      imageSize.width = Math.floor(imageSize.width * scalar);
      imageSize.height = Math.floor(imageSize.height * scalar);
      canvasSize = calculateCanvasSize(imageSize, aspectRatio, zoom);
    }
  }

  const canvas = document.createElement('canvas');

  const canvasCenter = {
    x: canvasSize.width * 0.5,
    y: canvasSize.height * 0.5
  };

  const stage = {
    x: 0,
    y: 0,
    width: canvasSize.width,
    height: canvasSize.height,
    center: canvasCenter
  };

  const shouldLimit = typeof crop.scaleToFit === 'undefined' || crop.scaleToFit;

  const scale =
    zoom *
    getImageRectZoomFactor(
      imageSize,
      getCenteredCropRect(stage, aspectRatio),
      crop.rotation,
      shouldLimit ? crop.center : { x: 0.5, y: 0.5 }
    );

  // start drawing
  canvas.width = Math.round(canvasSize.width / scale);
  canvas.height = Math.round(canvasSize.height / scale);

  canvasCenter.x /= scale;
  canvasCenter.y /= scale;

  const imageOffset = {
    x: canvasCenter.x - imageSize.width * (crop.center ? crop.center.x : 0.5),
    y: canvasCenter.y - imageSize.height * (crop.center ? crop.center.y : 0.5)
  };

  const ctx = canvas.getContext('2d');
  if (background) {
    ctx.fillStyle = background;
    ctx.fillRect(0, 0, canvas.width, canvas.height);
  }

  // move to draw offset
  ctx.translate(canvasCenter.x, canvasCenter.y);
  ctx.rotate(crop.rotation || 0);

  // draw the image
  ctx.drawImage(
    bitmap,
    imageOffset.x - canvasCenter.x,
    imageOffset.y - canvasCenter.y,
    imageSize.width,
    imageSize.height
  );

  // get data from canvas
  const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height);

  // release canvas
  canvasRelease(canvas);

  // return data
  return imageData;
};

/**
 * Polyfill toBlob for Edge
 */
const IS_BROWSER = (() =>
  typeof window !== 'undefined' && typeof window.document !== 'undefined')();
if (IS_BROWSER) {
  if (!HTMLCanvasElement.prototype.toBlob) {
    Object.defineProperty(HTMLCanvasElement.prototype, 'toBlob', {
      value: function(callback, type, quality) {
        var dataURL = this.toDataURL(type, quality).split(',')[1];
        setTimeout(function() {
          var binStr = atob(dataURL);
          var len = binStr.length;
          var arr = new Uint8Array(len);
          for (var i = 0; i < len; i++) {
            arr[i] = binStr.charCodeAt(i);
          }
          callback(new Blob([arr], { type: type || 'image/png' }));
        });
      }
    });
  }
}

const canvasToBlob = (canvas, options, beforeCreateBlob = null) =>
  new Promise(resolve => {
    const promisedImage = beforeCreateBlob ? beforeCreateBlob(canvas) : canvas;
    Promise.resolve(promisedImage).then(canvas => {
      canvas.toBlob(resolve, options.type, options.quality);
    });
  });

const vectorMultiply = (v, amount) =>
  createVector$1(v.x * amount, v.y * amount);

const vectorAdd = (a, b) => createVector$1(a.x + b.x, a.y + b.y);

const vectorNormalize = v => {
  const l = Math.sqrt(v.x * v.x + v.y * v.y);
  if (l === 0) {
    return {
      x: 0,
      y: 0
    };
  }
  return createVector$1(v.x / l, v.y / l);
};

const vectorRotate = (v, radians, origin) => {
  const cos = Math.cos(radians);
  const sin = Math.sin(radians);
  const t = createVector$1(v.x - origin.x, v.y - origin.y);
  return createVector$1(
    origin.x + cos * t.x - sin * t.y,
    origin.y + sin * t.x + cos * t.y
  );
};

const createVector$1 = (x = 0, y = 0) => ({ x, y });

const getMarkupValue = (value, size, scalar = 1, axis) => {
  if (typeof value === 'string') {
    return parseFloat(value) * scalar;
  }
  if (typeof value === 'number') {
    return value * (axis ? size[axis] : Math.min(size.width, size.height));
  }
  return;
};

const getMarkupStyles = (markup, size, scale) => {
  const lineStyle = markup.borderStyle || markup.lineStyle || 'solid';
  const fill = markup.backgroundColor || markup.fontColor || 'transparent';
  const stroke = markup.borderColor || markup.lineColor || 'transparent';
  const strokeWidth = getMarkupValue(
    markup.borderWidth || markup.lineWidth,
    size,
    scale
  );
  const lineCap = markup.lineCap || 'round';
  const lineJoin = markup.lineJoin || 'round';
  const dashes =
    typeof lineStyle === 'string'
      ? ''
      : lineStyle.map(v => getMarkupValue(v, size, scale)).join(',');
  const opacity = markup.opacity || 1;
  return {
    'stroke-linecap': lineCap,
    'stroke-linejoin': lineJoin,
    'stroke-width': strokeWidth || 0,
    'stroke-dasharray': dashes,
    stroke,
    fill,
    opacity
  };
};

const isDefined = value => value != null;

const getMarkupRect = (rect, size, scalar = 1) => {
  let left =
    getMarkupValue(rect.x, size, scalar, 'width') ||
    getMarkupValue(rect.left, size, scalar, 'width');
  let top =
    getMarkupValue(rect.y, size, scalar, 'height') ||
    getMarkupValue(rect.top, size, scalar, 'height');
  let width = getMarkupValue(rect.width, size, scalar, 'width');
  let height = getMarkupValue(rect.height, size, scalar, 'height');
  let right = getMarkupValue(rect.right, size, scalar, 'width');
  let bottom = getMarkupValue(rect.bottom, size, scalar, 'height');

  if (!isDefined(top)) {
    if (isDefined(height) && isDefined(bottom)) {
      top = size.height - height - bottom;
    } else {
      top = bottom;
    }
  }

  if (!isDefined(left)) {
    if (isDefined(width) && isDefined(right)) {
      left = size.width - width - right;
    } else {
      left = right;
    }
  }

  if (!isDefined(width)) {
    if (isDefined(left) && isDefined(right)) {
      width = size.width - left - right;
    } else {
      width = 0;
    }
  }

  if (!isDefined(height)) {
    if (isDefined(top) && isDefined(bottom)) {
      height = size.height - top - bottom;
    } else {
      height = 0;
    }
  }

  return {
    x: left || 0,
    y: top || 0,
    width: width || 0,
    height: height || 0
  };
};

const setAttributes = (element, attr) =>
  Object.keys(attr).forEach(key => element.setAttribute(key, attr[key]));

const ns = 'http://www.w3.org/2000/svg';
const svg = (tag, attr) => {
  const element = document.createElementNS(ns, tag);
  if (attr) {
    setAttributes(element, attr);
  }
  return element;
};

const updateRect = element =>
  setAttributes(element, {
    ...element.rect,
    ...element.styles
  });

const updateEllipse = element => {
  const cx = element.rect.x + element.rect.width * 0.5;
  const cy = element.rect.y + element.rect.height * 0.5;
  const rx = element.rect.width * 0.5;
  const ry = element.rect.height * 0.5;
  return setAttributes(element, {
    cx,
    cy,
    rx,
    ry,
    ...element.styles
  });
};

const IMAGE_FIT_STYLE = {
  contain: 'xMidYMid meet',
  cover: 'xMidYMid slice'
};

const updateImage = (element, markup) => {
  setAttributes(element, {
    ...element.rect,
    ...element.styles,
    preserveAspectRatio: IMAGE_FIT_STYLE[markup.fit] || 'none'
  });
};

const TEXT_ANCHOR = {
  left: 'start',
  center: 'middle',
  right: 'end'
};

const updateText = (element, markup, size, scale) => {
  const fontSize = getMarkupValue(markup.fontSize, size, scale);
  const fontFamily = markup.fontFamily || 'sans-serif';
  const fontWeight = markup.fontWeight || 'normal';
  const textAlign = TEXT_ANCHOR[markup.textAlign] || 'start';

  setAttributes(element, {
    ...element.rect,
    ...element.styles,
    'stroke-width': 0,
    'font-weight': fontWeight,
    'font-size': fontSize,
    'font-family': fontFamily,
    'text-anchor': textAlign
  });

  // update text
  if (element.text !== markup.text) {
    element.text = markup.text;
    element.textContent = markup.text.length ? markup.text : ' ';
  }
};

const updateLine = (element, markup, size, scale) => {
  setAttributes(element, {
    ...element.rect,
    ...element.styles,
    fill: 'none'
  });

  const line = element.childNodes[0];
  const begin = element.childNodes[1];
  const end = element.childNodes[2];

  const origin = element.rect;

  const target = {
    x: element.rect.x + element.rect.width,
    y: element.rect.y + element.rect.height
  };

  setAttributes(line, {
    x1: origin.x,
    y1: origin.y,
    x2: target.x,
    y2: target.y
  });

  if (!markup.lineDecoration) return;

  begin.style.display = 'none';
  end.style.display = 'none';

  const v = vectorNormalize({
    x: target.x - origin.x,
    y: target.y - origin.y
  });

  const l = getMarkupValue(0.05, size, scale);

  if (markup.lineDecoration.indexOf('arrow-begin') !== -1) {
    const arrowBeginRotationPoint = vectorMultiply(v, l);
    const arrowBeginCenter = vectorAdd(origin, arrowBeginRotationPoint);
    const arrowBeginA = vectorRotate(origin, 2, arrowBeginCenter);
    const arrowBeginB = vectorRotate(origin, -2, arrowBeginCenter);

    setAttributes(begin, {
      style: 'display:block;',
      d: `M${arrowBeginA.x},${arrowBeginA.y} L${origin.x},${origin.y} L${
        arrowBeginB.x
      },${arrowBeginB.y}`
    });
  }

  if (markup.lineDecoration.indexOf('arrow-end') !== -1) {
    const arrowEndRotationPoint = vectorMultiply(v, -l);
    const arrowEndCenter = vectorAdd(target, arrowEndRotationPoint);
    const arrowEndA = vectorRotate(target, 2, arrowEndCenter);
    const arrowEndB = vectorRotate(target, -2, arrowEndCenter);

    setAttributes(end, {
      style: 'display:block;',
      d: `M${arrowEndA.x},${arrowEndA.y} L${target.x},${target.y} L${
        arrowEndB.x
      },${arrowEndB.y}`
    });
  }
};

const createShape = node => markup => svg(node, { id: markup.id });

const createImage = markup => {
  const shape = svg('image', {
    id: markup.id,
    'stroke-linecap': 'round',
    'stroke-linejoin': 'round',
    opacity: '0'
  });
  shape.onload = () => {
    shape.setAttribute('opacity', markup.opacity || 1);
  };
  shape.setAttributeNS(
    'http://www.w3.org/1999/xlink',
    'xlink:href',
    markup.src
  );
  return shape;
};

const createLine = markup => {
  const shape = svg('g', {
    id: markup.id,
    'stroke-linecap': 'round',
    'stroke-linejoin': 'round'
  });

  const line = svg('line');
  shape.appendChild(line);

  const begin = svg('path');
  shape.appendChild(begin);

  const end = svg('path');
  shape.appendChild(end);

  return shape;
};

const CREATE_TYPE_ROUTES = {
  image: createImage,
  rect: createShape('rect'),
  ellipse: createShape('ellipse'),
  text: createShape('text'),
  line: createLine
};

const UPDATE_TYPE_ROUTES = {
  rect: updateRect,
  ellipse: updateEllipse,
  image: updateImage,
  text: updateText,
  line: updateLine
};

const createMarkupByType = (type, markup) => CREATE_TYPE_ROUTES[type](markup);

const updateMarkupByType = (element, type, markup, size, scale) => {
  element.rect = getMarkupRect(markup, size, scale);
  element.styles = getMarkupStyles(markup, size, scale);
  UPDATE_TYPE_ROUTES[type](element, markup, size, scale);
};

const sortMarkupByZIndex = (a, b) => {
  if (a[1].zIndex > b[1].zIndex) {
    return 1;
  }
  if (a[1].zIndex < b[1].zIndex) {
    return -1;
  }
  return 0;
};

const cropSVG = (blob, crop, markup, options) =>
  new Promise(resolve => {
    const { background = null } = options;

    // load blob contents and wrap in crop svg
    const fr = new FileReader();
    fr.onloadend = () => {
      // get svg text
      const text = fr.result;

      // create element with svg and get size
      const original = document.createElement('div');
      original.style.cssText = `position:absolute;pointer-events:none;width:0;height:0;visibility:hidden;`;
      original.innerHTML = text;
      const originalNode = original.querySelector('svg');
      document.body.appendChild(original);

      // request bounding box dimensions
      const bBox = originalNode.getBBox();
      original.parentNode.removeChild(original);

      // get title
      const titleNode = original.querySelector('title');

      // calculate new heights and widths
      const viewBoxAttribute = originalNode.getAttribute('viewBox') || '';
      const widthAttribute = originalNode.getAttribute('width') || '';
      const heightAttribute = originalNode.getAttribute('height') || '';
      let width = parseFloat(widthAttribute) || null;
      let height = parseFloat(heightAttribute) || null;
      const widthUnits = (widthAttribute.match(/[a-z]+/) || [])[0] || '';
      const heightUnits = (heightAttribute.match(/[a-z]+/) || [])[0] || '';

      // create new size
      const viewBoxList = viewBoxAttribute.split(' ').map(parseFloat);
      const viewBox = viewBoxList.length
        ? {
            x: viewBoxList[0],
            y: viewBoxList[1],
            width: viewBoxList[2],
            height: viewBoxList[3]
          }
        : bBox;

      let imageWidth = width != null ? width : viewBox.width;
      let imageHeight = height != null ? height : viewBox.height;

      originalNode.style.overflow = 'visible';
      originalNode.setAttribute('width', imageWidth);
      originalNode.setAttribute('height', imageHeight);

      // markup
      let markupSVG = '';
      if (markup && markup.length) {
        const size = {
          width: imageWidth,
          height: imageHeight
        };
        markupSVG = markup.sort(sortMarkupByZIndex).reduce((prev, shape) => {
          const el = createMarkupByType(shape[0], shape[1]);
          updateMarkupByType(el, shape[0], shape[1], size);
          el.removeAttribute('id');
          if (el.getAttribute('opacity') === 1) {
            el.removeAttribute('opacity');
          }
          return prev + '\n' + el.outerHTML + '\n';
        }, '');
        markupSVG = `\n\n<g>${markupSVG.replace(/&nbsp;/g, ' ')}</g>\n\n`;
      }

      const aspectRatio = crop.aspectRatio || imageHeight / imageWidth;

      const canvasWidth = imageWidth;
      const canvasHeight = canvasWidth * aspectRatio;

      const shouldLimit =
        typeof crop.scaleToFit === 'undefined' || crop.scaleToFit;

      const canvasZoomFactor = getImageRectZoomFactor(
        {
          width: imageWidth,
          height: imageHeight
        },
        getCenteredCropRect(
          {
            width: canvasWidth,
            height: canvasHeight
          },
          aspectRatio
        ),
        crop.rotation,
        shouldLimit
          ? crop.center
          : {
              x: 0.5,
              y: 0.5
            }
      );

      const scale = crop.zoom * canvasZoomFactor;

      const rotation = crop.rotation * (180 / Math.PI);

      const canvasCenter = {
        x: canvasWidth * 0.5,
        y: canvasHeight * 0.5
      };

      const imageOffset = {
        x: canvasCenter.x - imageWidth * crop.center.x,
        y: canvasCenter.y - imageHeight * crop.center.y
      };

      const cropTransforms = [
        // rotate
        `rotate(${rotation} ${canvasCenter.x} ${canvasCenter.y})`,

        // scale
        `translate(${canvasCenter.x} ${canvasCenter.y})`,
        `scale(${scale})`,
        `translate(${-canvasCenter.x} ${-canvasCenter.y})`,

        // offset
        `translate(${imageOffset.x} ${imageOffset.y})`
      ];

      const flipTransforms = [
        `scale(${crop.flip.horizontal ? -1 : 1} ${
          crop.flip.vertical ? -1 : 1
        })`,
        `translate(${crop.flip.horizontal ? -imageWidth : 0} ${
          crop.flip.vertical ? -imageHeight : 0
        })`
      ];

      // crop
      const transformed = `<?xml version="1.0" encoding="UTF-8"?>
<svg width="${canvasWidth}${widthUnits}" height="${canvasHeight}${heightUnits}" 
viewBox="0 0 ${canvasWidth} ${canvasHeight}" ${
        background ? 'style="background:' + background + '" ' : ''
      }
preserveAspectRatio="xMinYMin"
xmlns:xlink="http://www.w3.org/1999/xlink"
xmlns="http://www.w3.org/2000/svg">
<!-- Generated by PQINA - https://pqina.nl/ -->
<title>${titleNode ? titleNode.textContent : ''}</title>
<g transform="${cropTransforms.join(' ')}">
<g transform="${flipTransforms.join(' ')}">
${originalNode.outerHTML}${markupSVG}
</g>
</g>
</svg>`;

      // create new svg file
      resolve(transformed);
    };

    fr.readAsText(blob);
  });

const objectToImageData = obj => {
  let imageData;
  try {
    imageData = new ImageData(obj.width, obj.height);
  } catch (e) {
    // IE + Old EDGE (tested on 12)
    const canvas = document.createElement('canvas');
    imageData = canvas.getContext('2d').createImageData(obj.width, obj.height);
  }
  imageData.data.set(obj.data);
  return imageData;
};

/* javascript-obfuscator:disable */
const TransformWorker = () => {
  // maps transform types to transform functions
  const TRANSFORMS = { resize, filter };

  // applies all image transforms to the image data array
  const applyTransforms = (transforms, imageData) => {
    transforms.forEach(transform => {
      imageData = TRANSFORMS[transform.type](imageData, transform.data);
    });
    return imageData;
  };

  // transform image hub
  const transform = (data, cb) => {
    let transforms = data.transforms;

    // if has filter and has resize, move filter to resize operation
    let filterTransform = null;
    transforms.forEach(transform => {
      if (transform.type === 'filter') {
        filterTransform = transform;
      }
    });
    if (filterTransform) {
      // find resize
      let resizeTransform = null;
      transforms.forEach(transform => {
        if (transform.type === 'resize') {
          resizeTransform = transform;
        }
      });

      if (resizeTransform) {
        // update resize operation
        resizeTransform.data.matrix = filterTransform.data;

        // remove filter
        transforms = transforms.filter(
          transform => transform.type !== 'filter'
        );
      }
    }

    cb(applyTransforms(transforms, data.imageData));
  };

  // eslint-disable-next-line no-restricted-globals
  self.onmessage = e => {
    transform(e.data.message, response => {
      // eslint-disable-next-line no-restricted-globals
      self.postMessage({ id: e.data.id, message: response }, [
        response.data.buffer
      ]);
    });
  };

  function applyFilterMatrix(index, data, matrix) {
    let i = 0,
      row = 0,
      c = 0.0,
      r = data[index] / 255,
      g = data[index + 1] / 255,
      b = data[index + 2] / 255,
      a = data[index + 3] / 255;
    for (; i < 4; i++) {
      row = 5 * i;
      c =
        (r * matrix[row] +
          g * matrix[row + 1] +
          b * matrix[row + 2] +
          a * matrix[row + 3] +
          matrix[row + 4]) *
        255;
      data[index + i] = Math.max(0, Math.min(c, 255));
    }
  }

  const identityMatrix = self.JSON.stringify([
    1,
    0,
    0,
    0,
    0,
    0,
    1,
    0,
    0,
    0,
    0,
    0,
    1,
    0,
    0,
    0,
    0,
    0,
    1,
    0
  ]);
  function isIdentityMatrix(filter) {
    return self.JSON.stringify(filter || []) === identityMatrix;
  }

  function filter(imageData, matrix) {
    if (!matrix || isIdentityMatrix(matrix)) return imageData;

    const data = imageData.data;
    const l = data.length;

    const m11 = matrix[0];
    const m12 = matrix[1];
    const m13 = matrix[2];
    const m14 = matrix[3];
    const m15 = matrix[4];

    const m21 = matrix[5];
    const m22 = matrix[6];
    const m23 = matrix[7];
    const m24 = matrix[8];
    const m25 = matrix[9];

    const m31 = matrix[10];
    const m32 = matrix[11];
    const m33 = matrix[12];
    const m34 = matrix[13];
    const m35 = matrix[14];

    const m41 = matrix[15];
    const m42 = matrix[16];
    const m43 = matrix[17];
    const m44 = matrix[18];
    const m45 = matrix[19];

    let index = 0,
      r = 0.0,
      g = 0.0,
      b = 0.0,
      a = 0.0;

    for (; index < l; index += 4) {
      r = data[index] / 255;
      g = data[index + 1] / 255;
      b = data[index + 2] / 255;
      a = data[index + 3] / 255;
      data[index] = Math.max(
        0,
        Math.min((r * m11 + g * m12 + b * m13 + a * m14 + m15) * 255, 255)
      );
      data[index + 1] = Math.max(
        0,
        Math.min((r * m21 + g * m22 + b * m23 + a * m24 + m25) * 255, 255)
      );
      data[index + 2] = Math.max(
        0,
        Math.min((r * m31 + g * m32 + b * m33 + a * m34 + m35) * 255, 255)
      );
      data[index + 3] = Math.max(
        0,
        Math.min((r * m41 + g * m42 + b * m43 + a * m44 + m45) * 255, 255)
      );
    }

    return imageData;
  }

  function resize(imageData, data) {
    let { mode = 'contain', upscale = false, width, height, matrix } = data;

    // test if is identity matrix
    matrix = !matrix || isIdentityMatrix(matrix) ? null : matrix;

    // need at least a width or a height
    // also 0 is not a valid width or height
    if (!width && !height) {
      return filter(imageData, matrix);
    }

    // make sure all bounds are set
    if (width === null) {
      width = height;
    } else if (height === null) {
      height = width;
    }

    if (mode !== 'force') {
      let scalarWidth = width / imageData.width;
      let scalarHeight = height / imageData.height;
      let scalar = 1;

      if (mode === 'cover') {
        scalar = Math.max(scalarWidth, scalarHeight);
      } else if (mode === 'contain') {
        scalar = Math.min(scalarWidth, scalarHeight);
      }

      // if image is too small, exit here with original image
      if (scalar > 1 && upscale === false) {
        return filter(imageData, matrix);
      }

      width = imageData.width * scalar;
      height = imageData.height * scalar;
    }

    const originWidth = imageData.width;
    const originHeight = imageData.height;
    const targetWidth = Math.round(width);
    const targetHeight = Math.round(height);
    const inputData = imageData.data;
    const outputData = new Uint8ClampedArray(targetWidth * targetHeight * 4);
    const ratioWidth = originWidth / targetWidth;
    const ratioHeight = originHeight / targetHeight;
    const ratioWidthHalf = Math.ceil(ratioWidth * 0.5);
    const ratioHeightHalf = Math.ceil(ratioHeight * 0.5);

    for (let j = 0; j < targetHeight; j++) {
      for (let i = 0; i < targetWidth; i++) {
        let x2 = (i + j * targetWidth) * 4;
        let weight = 0;
        let weights = 0;
        let weightsAlpha = 0;
        let r = 0;
        let g = 0;
        let b = 0;
        let a = 0;
        let centerY = (j + 0.5) * ratioHeight;

        for (
          let yy = Math.floor(j * ratioHeight);
          yy < (j + 1) * ratioHeight;
          yy++
        ) {
          let dy = Math.abs(centerY - (yy + 0.5)) / ratioHeightHalf;
          let centerX = (i + 0.5) * ratioWidth;
          let w0 = dy * dy;

          for (
            let xx = Math.floor(i * ratioWidth);
            xx < (i + 1) * ratioWidth;
            xx++
          ) {
            let dx = Math.abs(centerX - (xx + 0.5)) / ratioWidthHalf;
            let w = Math.sqrt(w0 + dx * dx);

            if (w >= -1 && w <= 1) {
              weight = 2 * w * w * w - 3 * w * w + 1;

              if (weight > 0) {
                dx = 4 * (xx + yy * originWidth);

                let ref = inputData[dx + 3];
                a += weight * ref;
                weightsAlpha += weight;

                if (ref < 255) {
                  weight = (weight * ref) / 250;
                }

                r += weight * inputData[dx];
                g += weight * inputData[dx + 1];
                b += weight * inputData[dx + 2];
                weights += weight;
              }
            }
          }
        }

        outputData[x2] = r / weights;
        outputData[x2 + 1] = g / weights;
        outputData[x2 + 2] = b / weights;
        outputData[x2 + 3] = a / weightsAlpha;

        matrix && applyFilterMatrix(x2, outputData, matrix);
      }
    }

    return {
      data: outputData,
      width: targetWidth,
      height: targetHeight
    };
  }
};
/* javascript-obfuscator:enable */

const correctOrientation = (view, offset) => {
  // Missing 0x45786966 Marker? No Exif Header, stop here
  if (view.getUint32(offset + 4, false) !== 0x45786966) return;

  // next byte!
  offset += 4;

  // First 2bytes defines byte align of TIFF data.
  // If it is 0x4949="I I", it means "Intel" type byte align
  const intelByteAligned = view.getUint16((offset += 6), false) === 0x4949;
  offset += view.getUint32(offset + 4, intelByteAligned);

  const tags = view.getUint16(offset, intelByteAligned);
  offset += 2;

  // find Orientation tag
  for (let i = 0; i < tags; i++) {
    if (view.getUint16(offset + i * 12, intelByteAligned) === 0x0112) {
      view.setUint16(offset + i * 12 + 8, 1, intelByteAligned);
      return true;
    }
  }
  return false;
};

const readData = data => {
  const view = new DataView(data);

  // Every JPEG file starts from binary value '0xFFD8'
  // If it's not present, exit here
  if (view.getUint16(0) !== 0xffd8) return null;

  let offset = 2; // Start at 2 as we skipped two bytes (FFD8)
  let marker;
  let markerLength;
  let orientationCorrected = false;

  while (offset < view.byteLength) {
    marker = view.getUint16(offset, false);
    markerLength = view.getUint16(offset + 2, false) + 2;

    // Test if is APP and COM markers
    const isData = (marker >= 0xffe0 && marker <= 0xffef) || marker === 0xfffe;
    if (!isData) {
      break;
    }

    if (!orientationCorrected) {
      orientationCorrected = correctOrientation(view, offset, markerLength);
    }

    if (offset + markerLength > view.byteLength) {
      break;
    }

    offset += markerLength;
  }
  return data.slice(0, offset);
};

const getImageHead = file =>
  new Promise(resolve => {
    const reader = new FileReader();
    reader.onload = () => resolve(readData(reader.result) || null);
    reader.readAsArrayBuffer(file.slice(0, 256 * 1024));
  });

const getBlobBuilder = () => {
  return (window.BlobBuilder =
    window.BlobBuilder ||
    window.WebKitBlobBuilder ||
    window.MozBlobBuilder ||
    window.MSBlobBuilder);
};

const createBlob = (arrayBuffer, mimeType) => {
  const BB = getBlobBuilder();

  if (BB) {
    const bb = new BB();
    bb.append(arrayBuffer);
    return bb.getBlob(mimeType);
  }

  return new Blob([arrayBuffer], {
    type: mimeType
  });
};

const getUniqueId = () =>
  Math.random()
    .toString(36)
    .substr(2, 9);

const createWorker = fn => {
  const workerBlob = new Blob(['(', fn.toString(), ')()'], {
    type: 'application/javascript'
  });
  const workerURL = URL.createObjectURL(workerBlob);
  const worker = new Worker(workerURL);

  const trips = [];

  return {
    transfer: () => {}, // (message, cb) => {}
    post: (message, cb, transferList) => {
      const id = getUniqueId();
      trips[id] = cb;

      worker.onmessage = e => {
        const cb = trips[e.data.id];
        if (!cb) return;
        cb(e.data.message);
        delete trips[e.data.id];
      };

      worker.postMessage(
        {
          id,
          message
        },
        transferList
      );
    },
    terminate: () => {
      worker.terminate();
      URL.revokeObjectURL(workerURL);
    }
  };
};

const loadImage = url =>
  new Promise((resolve, reject) => {
    const img = new Image();
    img.onload = () => {
      resolve(img);
    };
    img.onerror = e => {
      reject(e);
    };
    img.src = url;
  });

const chain = funcs =>
  funcs.reduce(
    (promise, func) =>
      promise.then(result => func().then(Array.prototype.concat.bind(result))),
    Promise.resolve([])
  );

const canvasApplyMarkup = (canvas, markup) =>
  new Promise(resolve => {
    const size = {
      width: canvas.width,
      height: canvas.height
    };

    const ctx = canvas.getContext('2d');

    const drawers = markup.sort(sortMarkupByZIndex).map(item => () =>
      new Promise(resolve => {
        const result = TYPE_DRAW_ROUTES[item[0]](ctx, size, item[1], resolve);
        if (result) resolve();
      })
    );

    chain(drawers).then(() => resolve(canvas));
  });

const applyMarkupStyles = (ctx, styles) => {
  ctx.beginPath();
  ctx.lineCap = styles['stroke-linecap'];
  ctx.lineJoin = styles['stroke-linejoin'];
  ctx.lineWidth = styles['stroke-width'];
  if (styles['stroke-dasharray'].length) {
    ctx.setLineDash(styles['stroke-dasharray'].split(','));
  }
  ctx.fillStyle = styles['fill'];
  ctx.strokeStyle = styles['stroke'];
  ctx.globalAlpha = styles.opacity || 1;
};

const drawMarkupStyles = ctx => {
  ctx.fill();
  ctx.stroke();
  ctx.globalAlpha = 1;
};

const drawRect = (ctx, size, markup) => {
  const rect = getMarkupRect(markup, size);
  const styles = getMarkupStyles(markup, size);
  applyMarkupStyles(ctx, styles);
  ctx.rect(rect.x, rect.y, rect.width, rect.height);
  drawMarkupStyles(ctx, styles);
  return true;
};

const drawEllipse = (ctx, size, markup) => {
  const rect = getMarkupRect(markup, size);
  const styles = getMarkupStyles(markup, size);
  applyMarkupStyles(ctx, styles);

  const x = rect.x,
    y = rect.y,
    w = rect.width,
    h = rect.height,
    kappa = 0.5522848,
    ox = (w / 2) * kappa,
    oy = (h / 2) * kappa,
    xe = x + w,
    ye = y + h,
    xm = x + w / 2,
    ym = y + h / 2;

  ctx.moveTo(x, ym);
  ctx.bezierCurveTo(x, ym - oy, xm - ox, y, xm, y);
  ctx.bezierCurveTo(xm + ox, y, xe, ym - oy, xe, ym);
  ctx.bezierCurveTo(xe, ym + oy, xm + ox, ye, xm, ye);
  ctx.bezierCurveTo(xm - ox, ye, x, ym + oy, x, ym);

  drawMarkupStyles(ctx, styles);
  return true;
};

const drawImage = (ctx, size, markup, done) => {
  const rect = getMarkupRect(markup, size);
  const styles = getMarkupStyles(markup, size);
  applyMarkupStyles(ctx, styles);

  const image = new Image();
  image.onload = () => {
    if (markup.fit === 'cover') {
      const ar = rect.width / rect.height;
      const width = ar > 1 ? image.width : image.height * ar;
      const height = ar > 1 ? image.width / ar : image.height;
      const x = image.width * 0.5 - width * 0.5;
      const y = image.height * 0.5 - height * 0.5;
      ctx.drawImage(
        image,
        x,
        y,
        width,
        height,
        rect.x,
        rect.y,
        rect.width,
        rect.height
      );
    } else if (markup.fit === 'contain') {
      const scalar = Math.min(
        rect.width / image.width,
        rect.height / image.height
      );
      const width = scalar * image.width;
      const height = scalar * image.height;
      const x = rect.x + rect.width * 0.5 - width * 0.5;
      const y = rect.y + rect.height * 0.5 - height * 0.5;
      ctx.drawImage(
        image,
        0,
        0,
        image.width,
        image.height,
        x,
        y,
        width,
        height
      );
    } else {
      ctx.drawImage(
        image,
        0,
        0,
        image.width,
        image.height,
        rect.x,
        rect.y,
        rect.width,
        rect.height
      );
    }

    drawMarkupStyles(ctx, styles);
    done();
  };
  image.src = markup.src;
};

const drawText = (ctx, size, markup) => {
  const rect = getMarkupRect(markup, size);
  const styles = getMarkupStyles(markup, size);
  applyMarkupStyles(ctx, styles);

  const fontSize = getMarkupValue(markup.fontSize, size);
  const fontFamily = markup.fontFamily || 'sans-serif';
  const fontWeight = markup.fontWeight || 'normal';
  const textAlign = markup.textAlign || 'left';

  ctx.font = `${fontWeight} ${fontSize}px ${fontFamily}`;
  ctx.textAlign = textAlign;
  ctx.fillText(markup.text, rect.x, rect.y);

  drawMarkupStyles(ctx, styles);
  return true;
};

const drawLine = (ctx, size, markup) => {
  const rect = getMarkupRect(markup, size);
  const styles = getMarkupStyles(markup, size);
  applyMarkupStyles(ctx, styles);

  ctx.beginPath();

  const origin = {
    x: rect.x,
    y: rect.y
  };

  const target = {
    x: rect.x + rect.width,
    y: rect.y + rect.height
  };

  ctx.moveTo(origin.x, origin.y);
  ctx.lineTo(target.x, target.y);

  const v = vectorNormalize({
    x: target.x - origin.x,
    y: target.y - origin.y
  });

  const l = 0.04 * Math.min(size.width, size.height);

  if (markup.lineDecoration.indexOf('arrow-begin') !== -1) {
    const arrowBeginRotationPoint = vectorMultiply(v, l);
    const arrowBeginCenter = vectorAdd(origin, arrowBeginRotationPoint);
    const arrowBeginA = vectorRotate(origin, 2, arrowBeginCenter);
    const arrowBeginB = vectorRotate(origin, -2, arrowBeginCenter);

    ctx.moveTo(arrowBeginA.x, arrowBeginA.y);
    ctx.lineTo(origin.x, origin.y);
    ctx.lineTo(arrowBeginB.x, arrowBeginB.y);
  }
  if (markup.lineDecoration.indexOf('arrow-end') !== -1) {
    const arrowEndRotationPoint = vectorMultiply(v, -l);
    const arrowEndCenter = vectorAdd(target, arrowEndRotationPoint);
    const arrowEndA = vectorRotate(target, 2, arrowEndCenter);
    const arrowEndB = vectorRotate(target, -2, arrowEndCenter);

    ctx.moveTo(arrowEndA.x, arrowEndA.y);
    ctx.lineTo(target.x, target.y);
    ctx.lineTo(arrowEndB.x, arrowEndB.y);
  }

  // ctx.stroke();
  // ctx.globalAlpha = 1;

  drawMarkupStyles(ctx, styles);
  return true;
};

const TYPE_DRAW_ROUTES = {
  rect: drawRect,
  ellipse: drawEllipse,
  image: drawImage,
  text: drawText,
  line: drawLine
};

const imageDataToCanvas = imageData => {
  const image = document.createElement('canvas');
  image.width = imageData.width;
  image.height = imageData.height;
  const ctx = image.getContext('2d');
  ctx.putImageData(imageData, 0, 0);
  return image;
};

const transformImage = (file, instructions, options = {}) =>
  new Promise((resolve, reject) => {
    // if the file is not an image we do not have any business transforming it
    if (!file || !isImage$1(file))
      return reject({ status: 'not an image file', file });

    // get separate options for easier use
    const {
      stripImageHead,
      beforeCreateBlob,
      afterCreateBlob,
      canvasMemoryLimit
    } = options;

    // get crop
    const { crop, size, filter, markup, output } = instructions;

    // get exif orientation
    const orientation =
      instructions.image && instructions.image.orientation
        ? Math.max(1, Math.min(8, instructions.image.orientation))
        : null;

    // compression quality 0 => 100
    const qualityAsPercentage = output && output.quality;
    const quality =
      qualityAsPercentage === null ? null : qualityAsPercentage / 100;

    // output format
    const type = (output && output.type) || null;

    // background color
    const background = (output && output.background) || null;

    // get transforms
    const transforms = [];

    // add resize transforms if set
    if (
      size &&
      (typeof size.width === 'number' || typeof size.height === 'number')
    ) {
      transforms.push({ type: 'resize', data: size });
    }

    // add filters
    if (filter && filter.length === 20) {
      transforms.push({ type: 'filter', data: filter });
    }

    // resolves with supplied blob
    const resolveWithBlob = blob => {
      const promisedBlob = afterCreateBlob ? afterCreateBlob(blob) : blob;
      Promise.resolve(promisedBlob).then(resolve);
    };

    // done
    const toBlob = (imageData, options) => {
      const canvas = imageDataToCanvas(imageData);
      const promisedCanvas = markup.length
        ? canvasApplyMarkup(canvas, markup)
        : canvas;
      Promise.resolve(promisedCanvas).then(canvas => {
        canvasToBlob(canvas, options, beforeCreateBlob)
          .then(blob => {
            // force release of canvas memory
            canvasRelease(canvas);

            // remove image head (default)
            if (stripImageHead) return resolveWithBlob(blob);

            // try to copy image head
            getImageHead(blob).then(imageHead => {
              // re-inject image head EXIF info in case of JPEG, as the image head is removed by canvas export
              if (imageHead !== null) {
                blob = new Blob([imageHead, blob.slice(20)], {
                  type: blob.type
                });
              }

              // done!
              resolveWithBlob(blob);
            });
          })
          .catch(reject);
      });
    };

    // if this is an svg and we want it to stay an svg
    if (/svg/.test(file.type) && type === null) {
      return cropSVG(file, crop, markup, { background }).then(text => {
        resolve(createBlob(text, 'image/svg+xml'));
      });
    }

    // get file url
    const url = URL.createObjectURL(file);

    // turn the file into an image
    loadImage(url)
      .then(image => {
        // url is no longer needed
        URL.revokeObjectURL(url);

        // draw to canvas and start transform chain
        const imageData = imageToImageData(image, orientation, crop, {
          canvasMemoryLimit,
          background
        });

        // determine the format of the blob that we will output
        const outputFormat = {
          quality,
          type: type || file.type
        };

        // no transforms necessary, we done!
        if (!transforms.length) {
          return toBlob(imageData, outputFormat);
        }

        // send to the transform worker to transform the blob on a separate thread
        const worker = createWorker(TransformWorker);
        worker.post(
          {
            transforms,
            imageData
          },
          response => {
            // finish up
            toBlob(objectToImageData(response), outputFormat);

            // stop worker
            worker.terminate();
          },
          [imageData.data.buffer]
        );
      })
      .catch(reject);
  });

const MARKUP_RECT = [
  'x',
  'y',
  'left',
  'top',
  'right',
  'bottom',
  'width',
  'height'
];

const toOptionalFraction = value =>
  typeof value === 'string' && /%/.test(value)
    ? parseFloat(value) / 100
    : value;

// adds default markup properties, clones markup
const prepareMarkup = markup => {
  const [type, props] = markup;

  return [
    type,
    {
      zIndex: 0,
      ...props,
      ...MARKUP_RECT.reduce((prev, curr) => {
        prev[curr] = toOptionalFraction(props[curr]);
        return prev;
      }, {})
    }
  ];
};

/**
 * Polyfill Edge and IE when in Browser
 */
if (typeof window !== 'undefined' && typeof window.document !== 'undefined') {
  if (!HTMLCanvasElement.prototype.toBlob) {
    Object.defineProperty(HTMLCanvasElement.prototype, 'toBlob', {
      value: function(cb, type, quality) {
        const canvas = this;
        setTimeout(() => {
          const dataURL = canvas.toDataURL(type, quality).split(',')[1];
          const binStr = atob(dataURL);
          let index = binStr.length;
          const data = new Uint8Array(index);
          while (index--) {
            data[index] = binStr.charCodeAt(index);
          }
          cb(new Blob([data], { type: type || 'image/png' }));
        });
      }
    });
  }
}

const isBrowser =
  typeof window !== 'undefined' && typeof window.document !== 'undefined';
const isIOS =
  isBrowser && /iPad|iPhone|iPod/.test(navigator.userAgent) && !window.MSStream;

/**
 * Image Transform Plugin
 */
const plugin = ({ addFilter, utils }) => {
  const { Type, forin, getFileFromBlob, isFile } = utils;

  /**
   * Helper functions
   */

  // valid transforms (in correct order)
  const TRANSFORM_LIST = ['crop', 'resize', 'filter', 'markup', 'output'];

  const createVariantCreator = updateMetadata => (transform, file, metadata) =>
    transform(file, updateMetadata ? updateMetadata(metadata) : metadata);

  const isDefaultCrop = crop =>
    crop.aspectRatio === null &&
    crop.rotation === 0 &&
    crop.zoom === 1 &&
    crop.center &&
    crop.center.x === 0.5 &&
    crop.center.y === 0.5 &&
    crop.flip &&
    crop.flip.horizontal === false &&
    crop.flip.vertical === false;

  /**
   * Filters
   */
  addFilter(
    'SHOULD_PREPARE_OUTPUT',
    (shouldPrepareOutput, { query }) =>
      new Promise(resolve => {
        // If is not async should prepare now
        resolve(!query('IS_ASYNC'));
      })
  );

  // subscribe to file transformations
  addFilter(
    'PREPARE_OUTPUT',
    (file, { query, item }) =>
      new Promise(resolve => {
        // if the file is not an image we do not have any business transforming it
        if (
          !isFile(file) ||
          !isImage(file) ||
          !query('GET_ALLOW_IMAGE_TRANSFORM') ||
          item.archived
        ) {
          return resolve(file);
        }

        // get variants
        const variants = [];

        // add original file
        if (query('GET_IMAGE_TRANSFORM_VARIANTS_INCLUDE_ORIGINAL')) {
          variants.push(
            () =>
              new Promise(resolve => {
                resolve({
                  name: query('GET_IMAGE_TRANSFORM_VARIANTS_ORIGINAL_NAME'),
                  file
                });
              })
          );
        }

        // add default output version if output default set to true or if no variants defined
        if (query('GET_IMAGE_TRANSFORM_VARIANTS_INCLUDE_DEFAULT')) {
          variants.push(
            (transform, file, metadata) =>
              new Promise(resolve => {
                transform(file, metadata).then(file =>
                  resolve({
                    name: query('GET_IMAGE_TRANSFORM_VARIANTS_DEFAULT_NAME'),
                    file
                  })
                );
              })
          );
        }

        // get other variants
        const variantsDefinition = query('GET_IMAGE_TRANSFORM_VARIANTS') || {};
        forin(variantsDefinition, (key, fn) => {
          const createVariant = createVariantCreator(fn);
          variants.push(
            (transform, file, metadata) =>
              new Promise(resolve => {
                createVariant(transform, file, metadata).then(file =>
                  resolve({ name: key, file })
                );
              })
          );
        });

        // output format (quality 0 => 100)
        const qualityAsPercentage = query('GET_IMAGE_TRANSFORM_OUTPUT_QUALITY');
        const qualityMode = query('GET_IMAGE_TRANSFORM_OUTPUT_QUALITY_MODE');
        const quality =
          qualityAsPercentage === null ? null : qualityAsPercentage / 100;
        const type = query('GET_IMAGE_TRANSFORM_OUTPUT_MIME_TYPE');
        const clientTransforms =
          query('GET_IMAGE_TRANSFORM_CLIENT_TRANSFORMS') || TRANSFORM_LIST;

        // update transform metadata object
        item.setMetadata(
          'output',
          {
            type,
            quality,
            client: clientTransforms
          },
          true
        );

        // the function that is used to apply the file transformations
        const transform = (file, metadata) =>
          new Promise((resolve, reject) => {
            const filteredMetadata = { ...metadata };

            Object.keys(filteredMetadata)
              .filter(instruction => instruction !== 'exif')
              .forEach(instruction => {
                // if not in list, remove from object, the instruction will be handled by the server
                if (clientTransforms.indexOf(instruction) === -1) {
                  delete filteredMetadata[instruction];
                }
              });

            const {
              resize,
              exif,
              output,
              crop,
              filter,
              markup
            } = filteredMetadata;

            const instructions = {
              image: {
                orientation: exif ? exif.orientation : null
              },
              output:
                output &&
                (output.type ||
                  typeof output.quality === 'number' ||
                  output.background)
                  ? {
                      type: output.type,
                      quality:
                        typeof output.quality === 'number'
                          ? output.quality * 100
                          : null,
                      background:
                        output.background ||
                        query('GET_IMAGE_TRANSFORM_CANVAS_BACKGROUND_COLOR') ||
                        null
                    }
                  : undefined,
              size:
                resize && (resize.size.width || resize.size.height)
                  ? {
                      mode: resize.mode,
                      upscale: resize.upscale,
                      ...resize.size
                    }
                  : undefined,
              crop:
                crop && !isDefaultCrop(crop)
                  ? {
                      ...crop
                    }
                  : undefined,
              markup: markup && markup.length ? markup.map(prepareMarkup) : [],
              filter
            };

            if (instructions.output) {
              // determine if file type will change
              const willChangeType = output.type
                ? // type set
                  output.type !== file.type
                : // type not set
                  false;

              const canChangeQuality = /\/jpe?g$/.test(file.type);
              const willChangeQuality =
                output.quality !== null
                  ? // quality set
                    canChangeQuality && qualityMode === 'always'
                  : // quality not set
                    false;

              // determine if file data will be modified
              const willModifyImageData = !!(
                instructions.size ||
                instructions.crop ||
                instructions.filter ||
                willChangeType ||
                willChangeQuality
              );

              // if we're not modifying the image data then we don't have to modify the output
              if (!willModifyImageData) return resolve(file);
            }

            const options = {
              beforeCreateBlob: query('GET_IMAGE_TRANSFORM_BEFORE_CREATE_BLOB'),
              afterCreateBlob: query('GET_IMAGE_TRANSFORM_AFTER_CREATE_BLOB'),
              canvasMemoryLimit: query(
                'GET_IMAGE_TRANSFORM_CANVAS_MEMORY_LIMIT'
              ),
              stripImageHead: query(
                'GET_IMAGE_TRANSFORM_OUTPUT_STRIP_IMAGE_HEAD'
              )
            };

            transformImage(file, instructions, options)
              .then(blob => {
                // set file object
                const out = getFileFromBlob(
                  blob,
                  // rename the original filename to match the mime type of the output image
                  renameFileToMatchMimeType(
                    file.name,
                    getValidOutputMimeType(blob.type)
                  )
                );

                resolve(out);
              })
              .catch(reject);
          });

        // start creating variants
        const variantPromises = variants.map(create =>
          create(transform, file, item.getMetadata())
        );

        // wait for results
        Promise.all(variantPromises).then(files => {
          // if single file object in array, return the single file object else, return array of
          resolve(
            files.length === 1 && files[0].name === null
              ? // return the File object
                files[0].file
              : // return an array of files { name:'name', file:File }
                files
          );
        });
      })
  );

  // Expose plugin options
  return {
    options: {
      allowImageTransform: [true, Type.BOOLEAN],

      // null, 'image/jpeg', 'image/png'
      imageTransformOutputMimeType: [null, Type.STRING],

      // null, 0 - 100
      imageTransformOutputQuality: [null, Type.INT],

      // set to false to copy image exif data to output
      imageTransformOutputStripImageHead: [true, Type.BOOLEAN],

      // only apply transforms in this list
      imageTransformClientTransforms: [null, Type.ARRAY],

      // only apply output quality when a transform is required
      imageTransformOutputQualityMode: ['always', Type.STRING],
      // 'always'
      // 'optional'
      // 'mismatch' (future feature, only applied if quality differs from input)

      // get image transform variants
      imageTransformVariants: [null, Type.OBJECT],

      // should we post the default transformed file
      imageTransformVariantsIncludeDefault: [true, Type.BOOLEAN],

      // which name to prefix the default transformed file with
      imageTransformVariantsDefaultName: [null, Type.STRING],

      // should we post the original file
      imageTransformVariantsIncludeOriginal: [false, Type.BOOLEAN],

      // which name to prefix the original file with
      imageTransformVariantsOriginalName: ['original_', Type.STRING],

      // called before creating the blob, receives canvas, expects promise resolve with canvas
      imageTransformBeforeCreateBlob: [null, Type.FUNCTION],

      // expects promise resolved with blob
      imageTransformAfterCreateBlob: [null, Type.FUNCTION],

      // canvas memory limit
      imageTransformCanvasMemoryLimit: [
        isBrowser && isIOS ? 4096 * 4096 : null,
        Type.INT
      ],

      // background image of the output canvas
      imageTransformCanvasBackgroundColor: [null, Type.STRING]
    }
  };
};

// fire pluginloaded event if running in browser, this allows registering the plugin when using async script tags
if (isBrowser) {
  document.dispatchEvent(
    new CustomEvent('FilePond:pluginloaded', { detail: plugin })
  );
}

export default plugin;
