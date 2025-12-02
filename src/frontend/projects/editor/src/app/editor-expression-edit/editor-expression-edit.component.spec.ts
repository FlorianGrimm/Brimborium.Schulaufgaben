import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SAExpression, SAExpressionNode } from 'schulaufgaben';

import {
  EditorExpressionEditComponent,
  evaluateExpression,
  evaluateExpressionNode,
  generateFunctionForExpression,
  generateFunctionForExpressionNode,
  EXPRESSION_FUNCTIONS,
  EXPRESSION_CATEGORIES,
} from './editor-expression-edit.component';

describe('EditorExpressionEditComponent', () => {
  let component: EditorExpressionEditComponent;
  let fixture: ComponentFixture<EditorExpressionEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditorExpressionEditComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(EditorExpressionEditComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('EXPRESSION_FUNCTIONS', () => {
    it('should have all required categories', () => {
      const categories = new Set(EXPRESSION_FUNCTIONS.map(f => f.category));
      expect(categories.has('math')).toBeTrue();
      expect(categories.has('comparison')).toBeTrue();
      expect(categories.has('logic')).toBeTrue();
      expect(categories.has('string')).toBeTrue();
      expect(categories.has('value')).toBeTrue();
      expect(categories.has('reference')).toBeTrue();
    });

    it('should have matching EXPRESSION_CATEGORIES', () => {
      const funcCategories = new Set(EXPRESSION_FUNCTIONS.map(f => f.category));
      const definedCategories = new Set(EXPRESSION_CATEGORIES.map(c => c.value));
      expect(funcCategories).toEqual(definedCategories);
    });
  });

  describe('getFunctionDef', () => {
    it('should return function definition by name', () => {
      const def = component.getFunctionDef('add');
      expect(def).toBeDefined();
      expect(def?.label).toBe('Add (+)');
      expect(def?.argCount).toBe('variable');
    });

    it('should return undefined for unknown function', () => {
      const def = component.getFunctionDef('unknown');
      expect(def).toBeUndefined();
    });
  });

  describe('getFunctionsByCategory', () => {
    it('should return functions filtered by category', () => {
      const mathFuncs = component.getFunctionsByCategory('math');
      expect(mathFuncs.length).toBeGreaterThan(0);
      expect(mathFuncs.every(f => f.category === 'math')).toBeTrue();
    });
  });

  describe('isVariableArgs', () => {
    it('should return true for variable arg functions', () => {
      expect(component.isVariableArgs('add')).toBeTrue();
      expect(component.isVariableArgs('multiply')).toBeTrue();
      expect(component.isVariableArgs('and')).toBeTrue();
    });

    it('should return false for fixed arg functions', () => {
      expect(component.isVariableArgs('subtract')).toBeFalse();
      expect(component.isVariableArgs('divide')).toBeFalse();
      expect(component.isVariableArgs('number')).toBeFalse();
    });
  });

  describe('getMinArgs', () => {
    it('should return minimum args for variable functions', () => {
      expect(component.getMinArgs('add')).toBe(2);
      expect(component.getMinArgs('and')).toBe(2);
    });

    it('should return fixed arg count for fixed functions', () => {
      expect(component.getMinArgs('subtract')).toBe(2);
      expect(component.getMinArgs('not')).toBe(1);
      expect(component.getMinArgs('number')).toBe(0);
    });
  });

  describe('createNode', () => {
    it('should create a number node with default value', () => {
      const node = component.createNode('number');
      expect(node.FunctionName).toBe('number');
      expect(node.ValueText).toBe('0');
    });

    it('should create a string node with empty value', () => {
      const node = component.createNode('string');
      expect(node.FunctionName).toBe('string');
      expect(node.ValueText).toBe('');
    });

    it('should create a boolean node with true value', () => {
      const node = component.createNode('boolean');
      expect(node.FunctionName).toBe('boolean');
      expect(node.ValueText).toBe('true');
    });

    it('should create a reference node with empty id', () => {
      const node = component.createNode('reference');
      expect(node.FunctionName).toBe('reference');
      expect(node.ReferenceType).toBe('variable');
      expect(node.ReferenceId).toBe('');
    });

    it('should create an add node with 2 arguments', () => {
      const node = component.createNode('add');
      expect(node.FunctionName).toBe('add');
      expect(node.ListArgument?.length).toBe(2);
    });

    it('should create a not node with 1 argument', () => {
      const node = component.createNode('not');
      expect(node.FunctionName).toBe('not');
      expect(node.ListArgument?.length).toBe(1);
    });
  });

  describe('setRoot and clearRoot', () => {
    beforeEach(() => {
      component.inputExpression = { Root: null };
    });

    it('should set root node', () => {
      component.setRoot();
      const expression = component.expression$.getValue();
      expect(expression?.Root).toBeDefined();
      expect(expression?.Root?.FunctionName).toBe('add');
    });

    it('should clear root node', () => {
      component.setRoot();
      component.clearRoot();
      const expression = component.expression$.getValue();
      expect(expression?.Root).toBeNull();
    });
  });

  describe('canAddArgument', () => {
    it('should return true for variable arg nodes', () => {
      const node: SAExpressionNode = { FunctionName: 'add', ListArgument: [] };
      expect(component.canAddArgument(node)).toBeTrue();
    });

    it('should return false for fixed arg nodes', () => {
      const node: SAExpressionNode = { FunctionName: 'subtract', ListArgument: [] };
      expect(component.canAddArgument(node)).toBeFalse();
    });
  });

  describe('canRemoveArgument', () => {
    it('should return true if more than minArgs', () => {
      const node: SAExpressionNode = {
        FunctionName: 'add',
        ListArgument: [
          { FunctionName: 'number', ValueText: '1' },
          { FunctionName: 'number', ValueText: '2' },
          { FunctionName: 'number', ValueText: '3' },
        ]
      };
      expect(component.canRemoveArgument(node)).toBeTrue();
    });

    it('should return false if at minArgs', () => {
      const node: SAExpressionNode = {
        FunctionName: 'add',
        ListArgument: [
          { FunctionName: 'number', ValueText: '1' },
          { FunctionName: 'number', ValueText: '2' },
        ]
      };
      expect(component.canRemoveArgument(node)).toBeFalse();
    });
  });
});



describe('evaluateExpressionNode', () => {
  const lookup = (id: string) => {
    const values: Record<string, any> = {
      'x': 10,
      'y': 5,
      'name': 'hello',
      'flag': true,
    };
    return values[id];
  };

  describe('value nodes', () => {
    it('should evaluate string value', () => {
      const node: SAExpressionNode = { FunctionName: 'string', ValueText: 'test' };
      expect(evaluateExpressionNode(node, lookup)).toBe('test');
    });

    it('should evaluate number value', () => {
      const node: SAExpressionNode = { FunctionName: 'number', ValueText: '42' };
      expect(evaluateExpressionNode(node, lookup)).toBe(42);
    });

    it('should evaluate boolean true', () => {
      const node: SAExpressionNode = { FunctionName: 'boolean', ValueText: 'true' };
      expect(evaluateExpressionNode(node, lookup)).toBeTrue();
    });

    it('should evaluate boolean false', () => {
      const node: SAExpressionNode = { FunctionName: 'boolean', ValueText: 'false' };
      expect(evaluateExpressionNode(node, lookup)).toBeFalse();
    });

    it('should evaluate reference', () => {
      const node: SAExpressionNode = { FunctionName: 'reference', ReferenceId: 'x' };
      expect(evaluateExpressionNode(node, lookup)).toBe(10);
    });
  });

  describe('math operations', () => {
    it('should add numbers', () => {
      const node: SAExpressionNode = {
        FunctionName: 'add',
        ListArgument: [
          { FunctionName: 'number', ValueText: '3' },
          { FunctionName: 'number', ValueText: '4' },
          { FunctionName: 'number', ValueText: '5' },
        ]
      };
      expect(evaluateExpressionNode(node, lookup)).toBe(12);
    });

    it('should subtract numbers', () => {
      const node: SAExpressionNode = {
        FunctionName: 'subtract',
        ListArgument: [
          { FunctionName: 'number', ValueText: '10' },
          { FunctionName: 'number', ValueText: '3' },
        ]
      };
      expect(evaluateExpressionNode(node, lookup)).toBe(7);
    });

    it('should multiply numbers', () => {
      const node: SAExpressionNode = {
        FunctionName: 'multiply',
        ListArgument: [
          { FunctionName: 'number', ValueText: '2' },
          { FunctionName: 'number', ValueText: '3' },
          { FunctionName: 'number', ValueText: '4' },
        ]
      };
      expect(evaluateExpressionNode(node, lookup)).toBe(24);
    });

    it('should divide numbers', () => {
      const node: SAExpressionNode = {
        FunctionName: 'divide',
        ListArgument: [
          { FunctionName: 'number', ValueText: '20' },
          { FunctionName: 'number', ValueText: '4' },
        ]
      };
      expect(evaluateExpressionNode(node, lookup)).toBe(5);
    });

    it('should return 0 for division by zero', () => {
      const node: SAExpressionNode = {
        FunctionName: 'divide',
        ListArgument: [
          { FunctionName: 'number', ValueText: '10' },
          { FunctionName: 'number', ValueText: '0' },
        ]
      };
      expect(evaluateExpressionNode(node, lookup)).toBe(0);
    });
  });

  describe('comparison operations', () => {
    it('should evaluate equals', () => {
      const node: SAExpressionNode = {
        FunctionName: 'equals',
        ListArgument: [
          { FunctionName: 'number', ValueText: '5' },
          { FunctionName: 'number', ValueText: '5' },
        ]
      };
      expect(evaluateExpressionNode(node, lookup)).toBeTrue();
    });

    it('should evaluate greater', () => {
      const node: SAExpressionNode = {
        FunctionName: 'greater',
        ListArgument: [
          { FunctionName: 'number', ValueText: '10' },
          { FunctionName: 'number', ValueText: '5' },
        ]
      };
      expect(evaluateExpressionNode(node, lookup)).toBeTrue();
    });

    it('should evaluate less', () => {
      const node: SAExpressionNode = {
        FunctionName: 'less',
        ListArgument: [
          { FunctionName: 'number', ValueText: '3' },
          { FunctionName: 'number', ValueText: '5' },
        ]
      };
      expect(evaluateExpressionNode(node, lookup)).toBeTrue();
    });
  });

  describe('logical operations', () => {
    it('should evaluate and', () => {
      const node: SAExpressionNode = {
        FunctionName: 'and',
        ListArgument: [
          { FunctionName: 'boolean', ValueText: 'true' },
          { FunctionName: 'boolean', ValueText: 'true' },
        ]
      };
      expect(evaluateExpressionNode(node, lookup)).toBeTrue();
    });

    it('should evaluate or', () => {
      const node: SAExpressionNode = {
        FunctionName: 'or',
        ListArgument: [
          { FunctionName: 'boolean', ValueText: 'false' },
          { FunctionName: 'boolean', ValueText: 'true' },
        ]
      };
      expect(evaluateExpressionNode(node, lookup)).toBeTrue();
    });

    it('should evaluate not', () => {
      const node: SAExpressionNode = {
        FunctionName: 'not',
        ListArgument: [
          { FunctionName: 'boolean', ValueText: 'true' },
        ]
      };
      expect(evaluateExpressionNode(node, lookup)).toBeFalse();
    });
  });

  describe('string operations', () => {
    it('should evaluate startswith', () => {
      const node: SAExpressionNode = {
        FunctionName: 'startswith',
        ListArgument: [
          { FunctionName: 'string', ValueText: 'hello world' },
          { FunctionName: 'string', ValueText: 'hello' },
        ]
      };
      expect(evaluateExpressionNode(node, lookup)).toBeTrue();
    });

    it('should evaluate contains', () => {
      const node: SAExpressionNode = {
        FunctionName: 'contains',
        ListArgument: [
          { FunctionName: 'string', ValueText: 'hello world' },
          { FunctionName: 'string', ValueText: 'wor' },
        ]
      };
      expect(evaluateExpressionNode(node, lookup)).toBeTrue();
    });

    it('should evaluate concat', () => {
      const node: SAExpressionNode = {
        FunctionName: 'concat',
        ListArgument: [
          { FunctionName: 'string', ValueText: 'hello' },
          { FunctionName: 'string', ValueText: ' ' },
          { FunctionName: 'string', ValueText: 'world' },
        ]
      };
      expect(evaluateExpressionNode(node, lookup)).toBe('hello world');
    });
  });
});

describe('evaluateExpression', () => {
  it('should return null for empty expression', () => {
    const expr: SAExpression = { Root: null };
    expect(evaluateExpression(expr, () => null)).toBeNull();
  });

  it('should evaluate expression with root', () => {
    const expr: SAExpression = {
      Root: {
        FunctionName: 'add',
        ListArgument: [
          { FunctionName: 'number', ValueText: '1' },
          { FunctionName: 'number', ValueText: '2' },
        ]
      }
    };
    expect(evaluateExpression(expr, () => null)).toBe(3);
  });
});

describe('generateFunctionForExpressionNode', () => {
  const lookup = (id: string) => {
    const values: Record<string, any> = { 'x': 10, 'y': 5 };
    return values[id];
  };

  it('should generate function for number', () => {
    const node: SAExpressionNode = { FunctionName: 'number', ValueText: '42' };
    const fn = generateFunctionForExpressionNode(node);
    expect(fn(lookup).value).toBe(42);
    expect(fn(lookup).error).toBeUndefined();
  });

  it('should generate function for add', () => {
    const node: SAExpressionNode = {
      FunctionName: 'add',
      ListArgument: [
        { FunctionName: 'number', ValueText: '3' },
        { FunctionName: 'reference', ReferenceId: 'x' },
      ]
    };
    const fn = generateFunctionForExpressionNode(node);
    expect(fn(lookup).value).toBe(13);
  });

  it('should return error for division by zero', () => {
    const node: SAExpressionNode = {
      FunctionName: 'divide',
      ListArgument: [
        { FunctionName: 'number', ValueText: '10' },
        { FunctionName: 'number', ValueText: '0' },
      ]
    };
    const fn = generateFunctionForExpressionNode(node);
    expect(fn(lookup).error).toBe('Division by zero');
  });

  it('should return error for unknown function', () => {
    const node: SAExpressionNode = { FunctionName: 'unknown' as any };
    const fn = generateFunctionForExpressionNode(node);
    expect(fn(lookup).error).toBe('unknown function');
  });
});

describe('generateFunctionForExpression', () => {
  it('should return error for empty expression', () => {
    const expr: SAExpression = { Root: null };
    const fn = generateFunctionForExpression(expr);
    expect(fn(() => null).error).toBe('No root node');
  });

  it('should generate function for expression with root', () => {
    const expr: SAExpression = {
      Root: {
        FunctionName: 'multiply',
        ListArgument: [
          { FunctionName: 'number', ValueText: '2' },
          { FunctionName: 'number', ValueText: '3' },
        ]
      }
    };
    const fn = generateFunctionForExpression(expr);
    expect(fn(() => null).value).toBe(6);
  });
});