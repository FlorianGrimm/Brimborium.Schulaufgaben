import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';

export type GlobalCommand<Args=any> = {
  Name: string;
  IsEnabled$: BehaviorSubject<boolean>;
  CanExecute: (args: Args) => boolean;
  Execute: (args: Args) => void;
};
export type ContextCommand<Args=any> = {
  Name: string;
  FilterForContext(context: Context<any>): boolean;
  IsEnabled$: BehaviorSubject<boolean>;
  Execute: (args: Args) => void;
};

export type Context<T=any> = {
  typename: string;
  value: T;
  values: undefined;
}|{
  typename: string;
  value: undefined;
  values: T[];
};

@Injectable({
  providedIn: 'root',
})
export class CommonCommandService {
  
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
  public executeGlobalCommand<Args=any>(name: string, args: Args) {
    const list = this.listGlobalCommand$.getValue();
    const command = list.find((v) => v.Name === name);
    if (command) {
      command.Execute(args);
    }
  }

  public readonly context$ = new BehaviorSubject<Context|null>(null);
  
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

  public executeContextCommand<Args=any>(name: string, args: Args) {
    const list = this.listContextCommand$.getValue();
    const command = list.find((v) => v.Name === name);
    if (command) {
      command.Execute(args);
    }
  }
}
