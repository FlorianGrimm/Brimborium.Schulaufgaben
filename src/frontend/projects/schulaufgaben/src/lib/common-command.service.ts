import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Subscription } from 'rxjs';
import { createSubjectObservable } from './createSubjectObservable';

export type GlobalCommand<Args = any> = {
  Name: string;
  IsEnabled$: BehaviorSubject<boolean>;
  CanExecute: (args: Args) => boolean;
  Execute: (args: Args) => void;
};
export type ContextCommand<Args = any> = {
  Name: string;
  FilterForContext(context: Context<any>): boolean;
  IsEnabled$: BehaviorSubject<boolean>;
  Execute: (args: Args) => void;
};

export interface Context<T = any> {
  name: string;
  typename: string;
  selection: ContextSelection<T>;
}
export type ContextSelection<T = any> = ({
  value: undefined;
  values: undefined;
} | {
  value: T;
  values: undefined;
} | {
  value: undefined;
  values: T[];
});

@Injectable({
  providedIn: 'root',
})
export class CommonCommandService {
  readonly subscriptions = new Subscription();

  constructor() {
  }

  public readonly listGlobalCommand$ = new BehaviorSubject<GlobalCommand[]>([]);

  public addGlobalCommand(command: GlobalCommand) {
    const list = this.listGlobalCommand$.getValue();
    const nextList = [...list, command];
    this.listGlobalCommand$.next(nextList);
  }
  public removeGlobalCommand(command: GlobalCommand) {
    const list = this.listGlobalCommand$.getValue();
    const nextList = list.filter((v) => v !== command);
    this.listGlobalCommand$.next(nextList);
  }
  public getGlobalCommand(name: string) {
    const list = this.listGlobalCommand$.getValue();
    return list.find((v) => v.Name === name);
  }
  public getGlobalCommand$(name: string) {
    return this.listGlobalCommand$.pipe(map((v) => v.find((v) => v.Name === name)));
  }
  public executeGlobalCommand<Args = any>(name: string, args: Args) {
    const list = this.listGlobalCommand$.getValue();
    const command = list.find((v) => v.Name === name);
    if (command) {
      command.Execute(args);
    }
  }

  public readonly listContext$ = new BehaviorSubject<Context[]>([]);
  public readonly context$ = createSubjectObservable({
    initialValue: undefined,
    observable: this.listContext$.pipe(map((v) => (0 < v.length) ? v[0] : undefined)),
    subscription: this.subscriptions
  });

  public setContext(context: Context|undefined) {
    if (context == null) {
      this.listContext$.next([]);
    } else {
      this.listContext$.next([context]);
    }
  }

  public pushContext(context: Context) {
    const list = this.listContext$.getValue();
    const nextList = [context, ...list];
    this.listContext$.next(nextList);
  }

  public popContext(): (Context | undefined) {
    const list = this.listContext$.getValue();
    if (list.length <= 0) {
      return undefined;
    } else {
      const result = list[0];
      const nextList = list.slice(1);
      this.listContext$.next(nextList);
      return result;
    }
  }

  public readonly listContextCommand$ = new BehaviorSubject<ContextCommand[]>([]);

  public addContextCommand(command: ContextCommand) {
    const list = this.listContextCommand$.getValue();
    const nextList = [...list, command];
    this.listContextCommand$.next(nextList);
  }
  public removeContextCommand(command: ContextCommand) {
    const list = this.listContextCommand$.getValue();
    const nextList = list.filter((v) => v !== command);
    this.listContextCommand$.next(nextList);
  }

  public getContextCommandForContext$(context: Context) {
    return this.listContextCommand$.pipe(
      map((listCommand) => listCommand.filter((v) => v.FilterForContext(context))));
  }

  public executeContextCommand<Args = any>(name: string, args: Args) {
    const list = this.listContextCommand$.getValue();
    const command = list.find((v) => v.Name === name);
    if (command) {
      command.Execute(args);
    }
  }
}
