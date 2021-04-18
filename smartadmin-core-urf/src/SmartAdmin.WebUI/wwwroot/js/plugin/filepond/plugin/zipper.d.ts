import { Filter, PluginOptions, ZipperCallback } from './types';
declare const FilepondZipper: (callback?: ZipperCallback) => ({ addFilter }: PluginOptions) => Filter;
export default FilepondZipper;
