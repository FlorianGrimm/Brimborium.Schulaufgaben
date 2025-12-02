import { TestBed } from '@angular/core/testing';

import { CommonCommandService } from './common-command.service';

describe('CommonCommandService', () => {
  let service: CommonCommandService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CommonCommandService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('pushContext', () => {
    expect(service.listContext$.getValue().length).toEqual(0);
    expect(service.context$.getValue()).toBeUndefined();

    const context1 = { name: 'test1', typename: 'test1', selection:{value: undefined, values: undefined} };
    service.pushContext(context1);
    expect(service.listContext$.getValue().length).toEqual(1);
    expect(service.context$.getValue()).toBe(context1);
    
    const context2 = { name: 'test2', typename: 'test2', selection:{value: undefined, values: undefined} };
    service.pushContext(context2);
    expect(service.listContext$.getValue().length).toEqual(2);
    expect(service.context$.getValue()).toBe(context2);

    const poppedContext2 = service.popContext();
    expect(poppedContext2?.name).toBe(context2.name);
    
    expect(service.listContext$.getValue().length).toEqual(1);
    expect(service.context$.getValue()).toBe(context1);

    const poppedContext1 = service.popContext();
    expect(poppedContext1?.name).toBe(context1.name);
    /*
    const poppedContext0 = service.popContext();
    expect(poppedContext0).toBeUndefined();
    expect(service.context$.getValue()).toBeUndefined();
    */
  });

  
  it('setContext', () => {
    expect(service.listContext$.getValue().length).toEqual(0);
    expect(service.context$.getValue()).toBeUndefined();

    const context1 = { name: 'test1', typename: 'test1', selection:{value: undefined, values: undefined} };
    service.setContext(context1);
    expect(service.listContext$.getValue().length).toEqual(1);
    expect(service.context$.getValue()).toBe(context1);
    
    const context2 = { name: 'test2', typename: 'test2', selection:{value: undefined, values: undefined} };
    service.setContext(context2);
    expect(service.listContext$.getValue().length).toEqual(2);
    expect(service.context$.getValue()).toBe(context2);

    const poppedContext2 = service.popContext();
    expect(poppedContext2?.name).toBe(context2.name);
    
    expect(service.listContext$.getValue().length).toEqual(1);
    expect(service.context$.getValue()).toBe(context1);

    const poppedContext0 = service.popContext();
    expect(poppedContext0).toBeUndefined();
    expect(service.context$.getValue()).toBeUndefined();

  });
});
