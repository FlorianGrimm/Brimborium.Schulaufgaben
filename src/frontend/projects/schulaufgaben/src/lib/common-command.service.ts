import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';

export type GlobalCommand<Args=any> = {
  name: string;
  isEnabled: () => boolean;
  execute: (args: Args) => void;
};
export type ContextCommand<Args=any> = {
  name: string;
  isEnabled: () => boolean;
  execute: (args: Args) => void;
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
    return list.find((v) => v.name === name);
  }
  public getGlobalCommand$(name: string) {
    return this.listGlobalCommand$.pipe(map((v) => v.find((v) => v.name === name)));
  }
  public executeGlobalCommand<Args=any>(name: string, args: Args) {
    const list = this.listGlobalCommand$.getValue();
    const command = list.find((v) => v.name === name);
    if (command) {
      command.execute(args);
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
  public getContextCommand(context: any, name: string) {
    const list = this.listContextCommand$.getValue();
    return list.find((v) => v.name === name);
  }
  public getContextCommand$(name: string) {
    return this.listContextCommand$.pipe(map((v) => v.find((v) => v.name === name)));
  }
  public executeContextCommand<Args=any>(name: string, args: Args) {
    const list = this.listContextCommand$.getValue();
    const command = list.find((v) => v.name === name);
    if (command) {
      command.execute(args);
    }
  }
}
