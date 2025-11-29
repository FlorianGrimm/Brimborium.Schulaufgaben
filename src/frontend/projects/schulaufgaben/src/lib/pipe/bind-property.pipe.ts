import { Pipe, PipeTransform } from '@angular/core';
import { bindProperty, ObjectPath, BoundObjectPath } from '../object-path';

@Pipe({
  name: 'boundProperty',
})
export class BindPropertyPipe<T, K extends keyof T> implements PipeTransform {

  transform(
    value: T|undefined|null, 
    opath:ObjectPath<T>, 
    property: K
  ): BoundObjectPath<T[K]>|undefined {
    if (value === undefined || value === null) {
      return undefined;
    }
    return undefined;
    //return bindProperty<T, K>(value, opath, property as unknown as keyof T);
  }

}
