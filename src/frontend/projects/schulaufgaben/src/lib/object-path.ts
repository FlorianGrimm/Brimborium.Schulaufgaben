import { map, Observable } from "rxjs";

export class ObjectPathError extends Error {
  constructor(message:string) {
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
    = BoundObjectPathValue<V>
    | BoundObjectPathUndefined<V>
    ;

export type BoundObjectPathValue<V> = {
    opath: ObjectPath<V>;
    value: V
}
export type BoundObjectPathUndefined<V> = {
    opath: ObjectPath<V>;
    value: undefined;
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

type ToBoundObjectPath<T> = T extends any ? BoundObjectPathValue<T> : never;


export function bindProperty<T extends any, K extends keyof T>(
    item: undefined | null,
    property: K
): undefined;
export function bindProperty<T extends any, K extends keyof T>(
    item: BoundObjectPath<T | undefined | null>,
    property: K
): BoundObjectPath<T[K]>;
/*
export function bindProperty<T extends any | undefined | null, K extends keyof T>(
    item: BoundObjectPathUndefined<T>,
    property: K
): BoundObjectPathUndefined<T[K]>;
*/
export function bindProperty<T extends any, K extends keyof T>(
    item: BoundObjectPath<T | undefined | null> | undefined | null,
    property: K
): BoundObjectPath<T[K]>
    // | BoundObjectPathValue<T[K]> 
    // | BoundObjectPathUndefined<T[K]> 
    | undefined {
    if (item === undefined || item === null) { return undefined; }
    const nextOPath: ObjectPathProperty<T[K]> = {
        type: "property",
        property: property as string,
        parent: item.opath
    };

    if (item.value === null || item.value === undefined) {
        const result: BoundObjectPathUndefined<T[K]> = {
            opath: nextOPath,
            value: undefined
        };
        return result;
    } else {
        const value: T[K] = item.value[property];
        const result: BoundObjectPathValue<T[K]> = {
            opath: nextOPath,
            value: value
        };
        return result;
    }
}

export function boundNotNull<T>(value: T | undefined | null) {
    return value!;
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
                if (current === undefined || current === null) {
                    throw new ObjectPathError(`not found ${length - level - 1}:${parentObjectPath.property}`);
                }
                
                current = current[parentObjectPath.property];
            } else if (parentObjectPath.type === "index") {
                if (current === undefined || current === null) {
                    throw new ObjectPathError(`not found ${length - level - 1}:${parentObjectPath.index}`);
                }
                current = current[parentObjectPath.index];
            } else if (parentObjectPath.type === "id") {
                if (current === undefined || current === null) {
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
                const parent = listParent[level];
                const nextParent = { ...parent, [parentObjectPath.property]: current };
                current = nextParent;
            } else if (parentObjectPath.type === "index") {
                const parent = listParent[level];
                const nextParent = (parent as any[]).map((v, i) => (i === parentObjectPath.index) ? current : v);
                current = nextParent;
            } else if (parentObjectPath.type === "id") {
                const parent = listParent[level];
                const nextParent = (parent as any[]).map((v) => (v.id === parentObjectPath.id) ? current : v);
                current = nextParent;
            } else {
                throw new ObjectPathError(`unexpected ${(parentObjectPath as any)?.type}`);
            }
        }
    }
    return root;
}
export function applyBoundPath2<T = any, V = any>(
    root: T,
    opath: ObjectPath<V>,
    value: V
): T {
    if (opath.type === "root") {
        return value as unknown as T;
    }
    const listParent = applyBoundPathStep1(root, opath.parent);
    if (listParent === undefined) { throw new ObjectPathError("not found"); }
    const result = applyBoundPathStep2(listParent, opath, value);
    return result as unknown as T;
}

function applyBoundPathStep1<T, V = any>(root: T, opath: ObjectPath<V>): undefined | (any[]) {
    if (opath.type === "root") {
        return [root as unknown as V];
    }
    const listResult = applyBoundPathStep1(root, opath.parent);
    if (undefined === listResult) { return undefined; }
    const itemParent: any = listResult[listResult.length - 1];

    if (opath.type === "property") {
        const item = (itemParent as any)[opath.property] as unknown as V;
        listResult.push(item);
        return listResult;
    }
    if (opath.type === "index") {
        if (!Array.isArray(itemParent)) { return undefined; }
        const item = (itemParent as V[])[opath.index];
        listResult.push(item);
        return listResult;
    }
    if (opath.type === "id") {
        if (!Array.isArray(itemParent)) { return undefined; }
        const list = (itemParent as V[]);
        for (let index = 0; index < list.length; index++) {
            const item = list[index];
            if ((item as { id: string }).id === opath.id) {
                listResult.push(item);
                return listResult;
            }
        }
        return undefined;
    }
    throw new ObjectPathError(`unexpected ${(opath as any)?.type}`);
}

function applyBoundPathStep2(listParent: any[], opath: ObjectPath<any>, value: any): any {
    const item: any = listParent.pop();
    if (opath.type === "root") {
        return value;
    }
    if (opath.type === "property") {
        const nextItem: any = { ...item, [opath.property]: value };
        const result = applyBoundPathStep2(listParent, opath.parent, nextItem);
        return result;
    }
    if (opath.type === "index") {
        if (!Array.isArray(item)) { return undefined; }
        const nextItem = (item as any[]).map((v, i) => (i === opath.index) ? value : v);
        const result = applyBoundPathStep2(listParent, opath.parent, nextItem);
        return result;
    }
    if (opath.type === "id") {
        if (!Array.isArray(item)) { return undefined; }
        const list = (item as any[]);
        const nextItem = list.map((v, i) => (v.id === opath.id) ? value : v);
        const result = applyBoundPathStep2(listParent, opath.parent, nextItem);
        return result;
    }

    throw new ObjectPathError(`unexpected ${(opath as any)?.type}`);
}

/*
export function boundProperty$<T, K extends keyof T & string>(
    item: Observable<T>,
    parent: ObjectPath<any>,
    property: K
) {
    const value: Observable<T[K]> = item.pipe(map<T, T[K]>((v) => v[property]));
    const opath: ObjectPathProperty = {
        property: property
    };
    const result: BoundObjectPath = {
        opath: opath,
        value: value
    };
    return result;
}
*/

export function randomUUID() {
    return crypto.randomUUID();
}