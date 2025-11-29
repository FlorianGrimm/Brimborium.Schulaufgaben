import { TestBed } from '@angular/core/testing';

import { CommonDocumentManagerService } from './common-document-manager.service';
import { SADocument } from './model';

describe('CommonDocumentManagerService', () => {
  let service: CommonDocumentManagerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CommonDocumentManagerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('set document null', () => {
    service.setDocumentState(null);
    expect(service.document$.getValue()).toBeNull();
  });

  it('set document value', () => {
    const document: SADocument = createDocument("test");
    service.setDocumentState(document);
    expect(service.document$.getValue()).toBe(document);
  });

  it('set add document', () => {
    const document1: SADocument = createDocument("test1");
    service.setDocumentState(document1);
    const document2: SADocument = createDocument("test2");
    service.pushDocumentState(document2, "test2");
    expect(service.document$.getValue()).toBe(document2);
    expect(service.listHistoryUndo$.getValue()).toEqual([{ document: document1, action: 'new/load' }, { document: document2, action: 'test2' }]);
    expect(service.listHistoryRedo$.getValue()).toEqual([]);
  });

    it('set add document', () => {
    const document1: SADocument = createDocument("test1");
    service.setDocumentState(document1);
    
    const document2: SADocument = {...document1, Name: "test2"};
    service.pushDocumentState(document2, "test2");
    
    const document3: SADocument = {...document2, Name: "test3", Description: "test3"};
    service.pushDocumentState(document3, "test3");

    expect(service.document$.getValue()).toEqual(document3);
    expect(service.listHistoryUndo$.getValue().length).toEqual(3);
    expect(service.listHistoryRedo$.getValue()).toEqual([]);

    expect(service.itemHistoryUndo$.getValue()?.document.Name).toEqual("test3");
    expect(service.itemHistoryRedo$.getValue()).toBeNull();
    
    service.undo();
    expect(service.listHistoryUndo$.getValue().length).toEqual(2);
    expect(service.listHistoryRedo$.getValue().length).toEqual(1);
    expect(service.itemHistoryUndo$.getValue()?.document.Name).toEqual("test2");
    expect(service.itemHistoryRedo$.getValue()?.document.Name).toEqual("test3");
    expect(service.document$.getValue()).toEqual(document2);
    
    service.undo();
    expect(service.document$.getValue()).toEqual(document1);
    expect(service.listHistoryUndo$.getValue().length).toEqual(1);
    expect(service.listHistoryRedo$.getValue().length).toEqual(2);
    expect(service.itemHistoryUndo$.getValue()!.document.Name).toEqual("test1");
    expect(service.itemHistoryRedo$.getValue()!.document.Name).toEqual("test2");
    
    service.redo();
    expect(service.document$.getValue()).toEqual(document2);
    expect(service.listHistoryUndo$.getValue().length).toEqual(2);
    expect(service.listHistoryRedo$.getValue().length).toEqual(1);
    expect(service.itemHistoryUndo$.getValue()!.document.Name).toEqual("test2");
    expect(service.itemHistoryRedo$.getValue()!.document.Name).toEqual("test3");
    /*
    */
  });

});
function createDocument(name:string): SADocument {
  return {
    Id: "00000000-0000-0000-0000-000000000000",
    Name: name,
    Description: "test",
    KindInteraction: "",
    ListMedia: [],
    Decoration: undefined,
    Interaction: undefined,
    Width: { Value: 1, Unit: 1, Name: "test" },
    Height: { Value: 1, Unit: 1, Name: "test" },
    DefinedHorizontal: [],
    DefinedVertical: [],
    DefinedColor: [],
    RulerHorizontal: [],
    RulerVertical: []
  };
}

