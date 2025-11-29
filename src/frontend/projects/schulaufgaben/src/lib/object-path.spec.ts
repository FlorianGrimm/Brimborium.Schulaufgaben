
import { applyBoundPath, bindProperty, bindRoot, ObjectPath, randomUUID } from './object-path';
import { SADocument, SANode } from './model';

describe('ObjectPath', () => {
    it('root level', () => {
        const document1 = createDocument("document1");
        const bp0 = bindRoot(document1,"document1");
        const nextDocument1 = { ...document1, Name: "nextName" };
        const document2: SADocument = applyBoundPath(document1, bp0.opath, nextDocument1);
        expect(document2.Name).toBe("nextName");
    });

      it('one level', () => {
        const document1 = createDocument("document1");
        const bp0 = bindRoot(document1,"document1");
        const bp1 = bindProperty(bp0, "Name");
        const document2: SADocument = applyBoundPath(document1, bp1.opath, "nextName");
        expect(document2.Name).toBe("nextName");
    });

    it('two level', () => {
        const document1: SADocument = {
            ...createDocument("document1"),
            Decoration: createNode("Decoration1"),
            Interaction: createNode("Interaction1")
        };
        const bp0 = bindRoot(document1,"document1");
        const bp1 = bindProperty(bp0, "Decoration");
        const bp2 = bindProperty(bp1, "Name");
        const document2: SADocument = applyBoundPath(document1, bp2.opath, "nextName");
        expect(document2.Decoration?.Name).toBe("nextName");
    });

    
    it('two level - undefined middle', () => {
        const document1 = createDocument("document1");
        const bp0 = bindRoot(document1,"document1");
        const bp1 = bindProperty(bp0, "Decoration");
        const bp2 = bindProperty(bp1, "Name");
        const document2: SADocument = applyBoundPath(document1, bp2.opath, "nextName");
        expect(document2.Decoration?.Name).toBe("nextName");
    });
});

function createDocument(name: string): SADocument {
    return {
        Id: randomUUID(),
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
function createNode(name: string): SANode {
    const result: SANode = {
        Id: randomUUID(),
        Name: name,
        /*
  Kind?: SANodeKind;
  ListItem?: SANode[];
  Position?: SANodePosition;
  Normal?: SANodeNormal;
  Flipped?: SANodeFlipped;
  Selected?: SANodeSelected;
        
        */
    };
    return result;
}