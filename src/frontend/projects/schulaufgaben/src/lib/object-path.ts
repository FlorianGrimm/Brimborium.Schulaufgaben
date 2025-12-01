import { map, Observable } from "rxjs";

export class ObjectPathError extends Error {
    constructor(message: string) {
        super(message);
        this.name = this.constructor.name;
    }
}

export type ObjectPath<PropertyValue = any>
    = ObjectPathRoot<PropertyValue>
    | ObjectPathProperty<PropertyValue>
    | ObjectPathIndex<PropertyValue>
    | ObjectPathId<PropertyValue>
    ;

export type ObjectPathRoot<PropertyValue = any> = {
    type: "root";
    parent: undefined;
    name: string;
};

export type ObjectPathProperty<PropertyValue = any> = {
    type: "property";
    property: string;
    parent: ObjectPath<any>;
};

export type ObjectPathId<PropertyValue = any> = {
    type: "id";
    id: string;
    parent: ObjectPath<any>;
}

export type ObjectPathArray<PropertyValue = any> = {
    type: "array";
    property: string;
    parent: ObjectPath<any>;
}

export type ObjectPathIndex<PropertyValue = any> = {
    type: "index";
    index: number;
    parent: ObjectPath<any>;
}
export type BoundObjectPath<V>
    = (BoundObjectPathValue<V>)
    | (BoundObjectPathUndefined<V> extends {value: undefined} ? BoundObjectPathUndefined<V> : never)
    | (BoundObjectPathNull<V> extends {value: null} ? BoundObjectPathNull<V> : never)
    ;

export type BoundObjectPathValue<V extends Exclude<any, undefined | null>> = {
    opath: ObjectPath<V>;
    value: V
}
export type BoundObjectPathUndefined<V> = {
    opath: ObjectPath<V>;
    value: undefined;
}
export type BoundObjectPathNull<V> = {
    opath: ObjectPath<V>;
    value: null;
}

export function bindRoot<T>(
    item: T,
    name: string
): BoundObjectPathValue<T> {
    const opath: ObjectPathRoot<T> = {
        type: "root",
        parent: undefined,
        name: name
    };
    const result: BoundObjectPathValue<T> = {
        opath: opath,
        value: item
    };
    return result;
}

export type ToBoundObjectPath<T> = T extends any ? BoundObjectPathValue<T> : never;


export function bindProperty<T extends any, K extends keyof TV, TV extends Exclude<T, undefined | null> = Exclude<T, undefined | null>>(
    item: undefined|null,
    property: K
): undefined;
export function bindProperty<T extends any, K extends keyof TV, TV extends Exclude<T, undefined | null> = Exclude<T, undefined | null>>(
    item: BoundObjectPath<T>,
    property: K
): BoundObjectPath<TV[K]>;
export function bindProperty<T extends any, K extends keyof TV, TV extends Exclude<T, undefined | null> = Exclude<T, undefined | null>>(
    item: BoundObjectPath<T> | null | undefined,
    property: K
): BoundObjectPath<TV[K]> | undefined {
    if (item === undefined || item === null) { return undefined; }
    const nextOPath: ObjectPathProperty<TV[K]> = {
        type: "property",
        property: property as string,
        parent: item.opath
    };

    if (item.value == null) {
        if (item.value === null) {
            const result: BoundObjectPathNull<TV[K]> = {
                opath: nextOPath,
                value: null
            };
            return result;
        } else {
            const result: BoundObjectPathUndefined<TV[K]> = {
                opath: nextOPath,
                value: undefined
            };
            return result;
        }
    } else {
        const value: TV[K] = (item.value as TV)[property];
        const result: BoundObjectPathValue<TV[K]> = {
            opath: nextOPath,
            value: value
        };
        return result;
    }
}

type ArrayElement<Type> = Type extends Array<infer Item> ? Item : never;

export function boundIndex<T extends Array<ArrayElement<T>>>(
    item: T,
    index: number,
    parent: ObjectPath<any>
): BoundObjectPathValue<ArrayElement<T>> {
    const value = item[index];
    const opath: ObjectPathIndex<ArrayElement<T>> = {
        type: "index",
        index: index,
        parent
    };
    const result: BoundObjectPathValue<ArrayElement<T>> = {
        opath: opath,
        value: value
    }
    return result;
}

export function bindIndex<T extends Array<ArrayElement<T>>, E extends ArrayElement<T> = ArrayElement<T>>(
    item: undefined,
    index: number
): undefined;
export function bindIndex<T extends Array<ArrayElement<T>>, E extends ArrayElement<T> = ArrayElement<T>>(
    item: BoundObjectPath<T>,
    index: number
): BoundObjectPath<E>;
export function bindIndex<T extends Array<ArrayElement<T>>, E extends ArrayElement<T> = ArrayElement<T>>(
    item: BoundObjectPath<T> | undefined,
    index: number
): BoundObjectPath<E> | undefined {
    if (item === undefined || item === null) { return undefined; }
    const nextOPath: ObjectPathIndex<E> = {
        type: "index",
        index: index,
        parent: item.opath
    };

    if (item.value == null) {
        if (item.value === null) {
            const result: BoundObjectPathNull<E> = {
                opath: nextOPath,
                value: null
            };
            return result;
        } else {
            const result: BoundObjectPathUndefined<E> = {
                opath: nextOPath,
                value: undefined
            };
            return result;
        }
    } else {
        const value: E = item.value[index] as E;
        const result: BoundObjectPathValue<E> = {
            opath: nextOPath,
            value: value
        };
        return result;
    }
}
export function applyBoundPath<T = any, V = any>(
    root: T,
    opath: ObjectPath<V>,
    value: V
): T {
    if (opath.type === "root") {
        return value as unknown as T;
    }
    const listObjectPath: ObjectPath<any>[] = [];
    {
        for (let parent: ObjectPath<any> | undefined = opath; parent !== undefined; parent = parent.parent) {
            listObjectPath.push(parent);
        }
    }
    const length = listObjectPath.length;
    const listParent: any[] = Array(length);
    {
        let current: any = root;
        for (let level = length - 1; level >= 0; level--) {
            const parentObjectPath = listObjectPath[level];
            if (parentObjectPath.type === "root") {
                current = root;
            } else if (parentObjectPath.type === "property") {
                if (current == null) {
                    throw new ObjectPathError(`not found ${length - level - 1}:${parentObjectPath.property}`);
                }

                current = current[parentObjectPath.property];
            } else if (parentObjectPath.type === "index") {
                if (current == null) {
                    throw new ObjectPathError(`not found ${length - level - 1}:${parentObjectPath.index}`);
                }
                current = current[parentObjectPath.index];
            } else if (parentObjectPath.type === "id") {
                if (current == null) {
                    throw new ObjectPathError(`not found ${length - level - 1}:${parentObjectPath.id}`);
                }
                current = current.find((v: any) => v.id === parentObjectPath.id);
            } else {
                throw new ObjectPathError(`unexpected ${(parentObjectPath as any)?.type}`);
            }
            listParent[level] = current;
        }
    }
    {
        let current: any = value;
        for (let level = 0; level < length; level++) {
            const parentObjectPath = listObjectPath[level];
            if (parentObjectPath.type === "root") {
                return current;
            } else if (parentObjectPath.type === "property") {
                const parent = listParent[level+1];
                const nextParent = { ...parent, [parentObjectPath.property]: current };
                current = nextParent;
            } else if (parentObjectPath.type === "index") {
                const parent = listParent[level+1];
                const nextParent = (parent as any[]).map((v, i) => (i === parentObjectPath.index) ? current : v);
                current = nextParent;
            } else if (parentObjectPath.type === "id") {
                const parent = listParent[level+1];
                const nextParent = (parent as any[]).map((v) => (v.id === parentObjectPath.id) ? current : v);
                current = nextParent;
            } else {
                throw new ObjectPathError(`unexpected ${(parentObjectPath as any)?.type}`);
            }
        }
    }
    return root;
}

export function randomUUID() {
    return crypto.randomUUID();
}