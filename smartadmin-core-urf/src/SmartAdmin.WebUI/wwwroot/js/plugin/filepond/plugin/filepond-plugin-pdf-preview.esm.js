/*!
 * FilePondPluginPdfPreview 1.0.2
 * Licensed under MIT, https://opensource.org/licenses/MIT/
 * Please visit undefined for details.
 */

/* eslint-disable */

const isPreviewablePdf = (file) => /pdf$/.test(file.type);


const createPdfView = (_) =>
  _.utils.createView({
    name: 'pdf-preview',
    tag: 'div',
    ignoreRect: true,
    create: ({ root, props }) => {
      // get item
      const item = root.query('GET_ITEM', { id: props.id });
      if (isPreviewablePdf(item.file)) {
          const numPdfPreviewHeight = root.query('GET_PDF_PREVIEW_HEIGHT');          
          root.ref.pdf = document.createElement('object');
          root.ref.pdf.setAttribute('height', numPdfPreviewHeight);
          root.ref.pdf.setAttribute('width', "100%");//320
          root.ref.pdf.setAttribute(
            'style',
            'position:absolute;left:0;right:0;margin:auto;padding-top:unset;' +
            ((numPdfPreviewHeight) ? ('height:' + numPdfPreviewHeight + 'px;') : '') 
                 
          );
          root.element.appendChild(root.ref.pdf);
      } 
    },
    write: _.utils.createRoute({
      DID_PDF_PREVIEW_LOAD: ({ root, props }) => {
        const { id } = props;

        // get item
        const item = root.query('GET_ITEM', { id: id });
        if (!item) return;

        let URL = window.URL || window.webkitURL;
        let blob = new Blob([item.file], { type: item.file.type });

        root.ref.pdf.type = item.file.type;
        if (isPreviewablePdf(item.file)) {
            const sPdfComponentExtraParams = root.query('GET_PDF_COMPONENT_EXTRA_PARAMS');
            root.ref.pdf.data = URL.createObjectURL(blob) + ((!sPdfComponentExtraParams)?"":("#?"+sPdfComponentExtraParams));
        }
        //else root.ref.pdf.src = URL.createObjectURL(blob);
        
        root.ref.pdf.addEventListener(
          'load',
          () => {
            if (isPreviewablePdf(item.file)) {
              root.dispatch('DID_UPDATE_PANEL_HEIGHT', {
                id: id,
                height: root.ref.pdf.scrollHeight,
              });
            }
          },
          false
        );
      },
    }),
  });

const createPdfWrapperView = (_) => {
  /**
   * Write handler for when preview container has been created
   */
  const didCreatePreviewContainer = ({ root, props }) => {
    const { id } = props;
    const item = root.query('GET_ITEM', id);
    if (!item) return;

    // the preview is now ready to be drawn
    root.dispatch('DID_PDF_PREVIEW_LOAD', {
      id,
    });
  };

  /**
   * Constructor
   */
  const create = ({ root, props }) => {
    const pdf = createPdfView(_);

    // append pdf presenter
    root.ref.pdf = root.appendChildView(
      root.createChildView(pdf, {
        id: props.id,
      })
    );
  };

  return _.utils.createView({
    name: 'pdf-preview-wrapper',
    create,
    write: _.utils.createRoute({
      // pdf preview stated
      DID_PDF_PREVIEW_CONTAINER_CREATE: didCreatePreviewContainer,
    }),
  });
};

/**
 * Pdf Preview Plugin
 */
const plugin = (fpAPI) => {
  const { addFilter, utils } = fpAPI;
  const { Type, createRoute } = utils;
  const pdfWrapperView = createPdfWrapperView(fpAPI);

  // called for each view that is created right after the 'create' method
  addFilter('CREATE_VIEW', (viewAPI) => {
    // get reference to created view
    const { is, view, query } = viewAPI;

    // only hook up to item view
    if (!is('file')) {
      return;
    }

    // create the pdf preview plugin
    const didLoadItem = ({ root, props }) => {
      const { id } = props;
      const item = query('GET_ITEM', id);

      if (
          !item ||
          item.archived ||
          ( !isPreviewablePdf(item.file))
      ) {
          return;
      } // set preview view

      // set preview view
      root.ref.pdfPreview = view.appendChildView(
        view.createChildView(pdfWrapperView, { id })
      );

      // now ready
      root.dispatch('DID_PDF_PREVIEW_CONTAINER_CREATE', { id });
    };

    // start writing
    view.registerWriter(
      createRoute(
        {
          DID_LOAD_ITEM: didLoadItem,
        },
        ({ root, props }) => {
          const { id } = props;
          const item = query('GET_ITEM', id);

          // don't do anything while not an pdf or hidden
          if (
            ( !isPreviewablePdf(item.file) ) ||
            root.rect.element.hidden
          )
            return;
        }
      )
    );
  });

  // expose plugin
  return {
    options: {
        
        allowPdfPreview: [true, Type.BOOLEAN],

        // Fixed PDF preview height
        pdfPreviewHeight: [320, Type.INT],

        // Extra params to pass to the pdf visulizer
        pdfComponentExtraParams: ['toolbar=0&navpanes=0&scrollbar=0&statusbar=0&zoom=0&messages=0&view=fitH&page=1', Type.STRING],
    },
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
