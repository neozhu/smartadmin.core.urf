interface TargetFilter {
    ADD_ITEMS: string;
}
export declare type FilterKey = keyof TargetFilter;
export interface ItemType extends File {
    _relativePath?: string;
}
declare type FilterCallback = (items: ItemType[]) => Promise<ItemType[]>;
declare type AddFilterCallback = (key: FilterKey, callback: FilterCallback) => void;
export interface PluginOptions {
    addFilter: AddFilterCallback;
}
export interface Filter {
    options: unknown;
}
export interface Metadata {
    percent: number;
    currentFile: string;
}
export declare type OnUpdateCallback = (metadata: Metadata) => void;
export declare type GeneratorCallback = (onUpdate?: OnUpdateCallback) => Promise<ItemType>;
export declare type ZipperCallback = (generators: GeneratorCallback[]) => unknown;
export declare class Item extends File implements ItemType {
    _relativePath?: string;
}
export {};
