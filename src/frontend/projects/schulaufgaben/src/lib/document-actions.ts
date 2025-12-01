import { map } from "rxjs";
import { SchulaufgabenEditorWebV1Service } from "./api-client.service";
import { SADocument, SANode } from "./model";

export function loadDocument(id: string, client: SchulaufgabenEditorWebV1Service) {
    return client.getAPIDocumentId(id).pipe(
        map((document) => normalizeDocument(document, id))
    );
}
export function createDocument(name: string): SADocument {
    const result: SADocument = {
        Id: crypto.randomUUID(),
        Name: name,
        Description: "",
        KindInteraction: "",
        ListMedia: [],
        Decoration: createSANode("Decoration"),
        Interaction: createSANode("Interaction"),
        Width: { Value: 1200, Unit: 1, Name: "PageWidth" },
        Height: { Value: 800, Unit: 1, Name: "PageHeight" },
        DefinedHorizontal: [],
        DefinedVertical: [],
        DefinedColor: [],
        RulerHorizontal: [],
        RulerVertical: []
    };
    return result;
}
export function normalizeDocument(document: SADocument, id?: string) {
    if (id){
        if(document.Id !== id){
            document.Id = id;
        }
    }
    if (document.ListMedia == null) {
        document.ListMedia = [];
    }
    if (document.Decoration == null) {
        document.Decoration = createSANode("Decoration");
    }
    if (document.Interaction == null) {
        document.Interaction = createSANode("Interaction");
    }
    if (document.Width == null) {
        document.Width = { Value: 1200, Unit: 1, Name: "PageWidth" };
    }
    if (document.Height == null) {
        document.Height = { Value: 800, Unit: 1, Name: "PageHeight" };
    }
    if (document.DefinedHorizontal == null) {
        document.DefinedHorizontal = [];
    }
    if (document.DefinedVertical == null) {
        document.DefinedVertical = [];
    }
    if (document.RulerHorizontal == null) {
        document.RulerHorizontal = [];
    }
    if (document.RulerVertical == null) {
        document.RulerVertical = [];
    }
    return document;
}
export function createSANode(name: string): SANode {
    const result: SANode = {
        Id: crypto.randomUUID(),
        Name: 'root',
        Kind: '',
        ListItem: [],
        Position: undefined,
        Normal: undefined,
        Flipped: undefined,
        Selected: undefined
    };
    return result;
}

