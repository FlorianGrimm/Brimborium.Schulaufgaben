import { Component, Input, output } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { SAExpression, SAExpressionNode, BaseComponent } from 'schulaufgaben';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { AsyncPipe, NgTemplateOutlet } from '@angular/common';

/** Argument type for expression functions */
export type ExpressionArgType = 'any' | 'number' | 'string' | 'boolean' | 'expression';

/** Expression function definition */
export interface ExpressionFunctionDef {
  value: string;
  label: string;
  argCount: number | 'variable'; // 'variable' means unlimited arguments (min 1)
  minArgs?: number; // minimum args for variable
  argTypes?: ExpressionArgType[]; // types for each argument position
  returnType: ExpressionArgType;
  category: 'math' | 'comparison' | 'logic' | 'string' | 'value' | 'reference';
}

/** Available expression functions */
export const EXPRESSION_FUNCTIONS: ExpressionFunctionDef[] = [
  // Math operations
  { value: 'add', label: 'Add (+)', argCount: 'variable', minArgs: 2, returnType: 'number', category: 'math' },
  { value: 'subtract', label: 'Subtract (-)', argCount: 2, returnType: 'number', category: 'math' },
  { value: 'multiply', label: 'Multiply (×)', argCount: 'variable', minArgs: 2, returnType: 'number', category: 'math' },
  { value: 'divide', label: 'Divide (÷)', argCount: 2, returnType: 'number', category: 'math' },

  // Comparison operations
  { value: 'equals', label: 'Equals (=)', argCount: 2, returnType: 'boolean', category: 'comparison' },
  { value: 'greater', label: 'Greater (>)', argCount: 2, returnType: 'boolean', category: 'comparison' },
  { value: 'less', label: 'Less (<)', argCount: 2, returnType: 'boolean', category: 'comparison' },
  { value: 'greaterorequal', label: 'Greater or Equal (≥)', argCount: 2, returnType: 'boolean', category: 'comparison' },
  { value: 'lessorequal', label: 'Less or Equal (≤)', argCount: 2, returnType: 'boolean', category: 'comparison' },

  // Logical operations
  { value: 'and', label: 'And (&&)', argCount: 'variable', minArgs: 2, returnType: 'boolean', category: 'logic' },
  { value: 'or', label: 'Or (||)', argCount: 'variable', minArgs: 2, returnType: 'boolean', category: 'logic' },
  { value: 'not', label: 'Not (!)', argCount: 1, returnType: 'boolean', category: 'logic' },

  // String operations
  { value: 'startswith', label: 'Starts With', argCount: 2, argTypes: ['string', 'string'], returnType: 'boolean', category: 'string' },
  { value: 'contains', label: 'Contains', argCount: 2, argTypes: ['string', 'string'], returnType: 'boolean', category: 'string' },
  { value: 'concat', label: 'Concatenate', argCount: 'variable', minArgs: 2, returnType: 'string', category: 'string' },

  // Value types (leaf nodes)
  { value: 'string', label: 'String Value', argCount: 0, returnType: 'string', category: 'value' },
  { value: 'number', label: 'Number Value', argCount: 0, returnType: 'number', category: 'value' },
  { value: 'boolean', label: 'Boolean Value', argCount: 0, returnType: 'boolean', category: 'value' },

  // Reference
  { value: 'reference', label: 'Reference', argCount: 0, returnType: 'any', category: 'reference' },
];

/** Get function categories for grouping in select */
export const EXPRESSION_CATEGORIES = [
  { value: 'math', label: 'Math' },
  { value: 'comparison', label: 'Comparison' },
  { value: 'logic', label: 'Logic' },
  { value: 'string', label: 'String' },
  { value: 'value', label: 'Value' },
  { value: 'reference', label: 'Reference' },
];

@Component({
  selector: 'app-editor-expression-edit',
  imports: [
    AsyncPipe,
    NgTemplateOutlet,
    FormsModule,
    DragDropModule,
    MatSelectModule,
    MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './editor-expression-edit.component.html',
  styleUrl: './editor-expression-edit.component.scss',
})
export class EditorExpressionEditComponent extends BaseComponent {
  public expression$ = new BehaviorSubject<SAExpression | null>(null);
  public readonly onExpressionChanged = output<SAExpression>();
  public readonly expressionFunctions = EXPRESSION_FUNCTIONS;
  public readonly expressionCategories = EXPRESSION_CATEGORIES;

  @Input()
  set inputExpression(value: SAExpression | undefined | null) {
    this.expression$.next(value ?? null);
  }

  constructor() {
    super();
  }

  /** Get function definition by name */
  getFunctionDef(functionName: string | null | undefined): ExpressionFunctionDef | undefined {
    return this.expressionFunctions.find(f => f.value === functionName);
  }

  /** Get functions by category */
  getFunctionsByCategory(category: string): ExpressionFunctionDef[] {
    return this.expressionFunctions.filter(f => f.category === category);
  }

  /** Check if function has variable arguments */
  isVariableArgs(functionName: string | null | undefined): boolean {
    const def = this.getFunctionDef(functionName);
    return def?.argCount === 'variable';
  }

  /** Get minimum arguments for a function */
  getMinArgs(functionName: string | null | undefined): number {
    const def = this.getFunctionDef(functionName);
    if (!def) return 0;
    if (def.argCount === 'variable') return def.minArgs ?? 1;
    return def.argCount;
  }

  /** Get expected argument count (or -1 for variable) */
  getArgCount(functionName: string | null | undefined): number {
    const def = this.getFunctionDef(functionName);
    if (!def) return 0;
    if (def.argCount === 'variable') return -1;
    return def.argCount;
  }

  /** Create a new node with default values */
  createNode(functionName: string = 'number'): SAExpressionNode {
    const def = this.getFunctionDef(functionName);
    const node: SAExpressionNode = {
      FunctionName: functionName,
    };

    if (functionName === 'string' || functionName === 'number' || functionName === 'boolean') {
      node.ValueText = functionName === 'number' ? '0' : (functionName === 'boolean' ? 'true' : '');
    } else if (functionName === 'reference') {
      node.ReferenceType = 'variable';
      node.ReferenceId = '';
    } else if (def && def.argCount !== 0) {
      const argCount = def.argCount === 'variable' ? (def.minArgs ?? 2) : def.argCount;
      node.ListArgument = Array.from({ length: argCount }, () => this.createNode('number'));
    }

    return node;
  }

  /** Set root node */
  setRoot(): void {
    const expression = this.expression$.getValue();
    if (expression) {
      const newExpression: SAExpression = {
        ...expression,
        Root: this.createNode('add'),
      };
      this.expression$.next(newExpression);
      this.onExpressionChanged.emit(newExpression);
    }
  }

  /** Clear root node */
  clearRoot(): void {
    const expression = this.expression$.getValue();
    if (expression) {
      const newExpression: SAExpression = {
        ...expression,
        Root: null,
      };
      this.expression$.next(newExpression);
      this.onExpressionChanged.emit(newExpression);
    }
  }

  /** Update a node at given path */
  updateNodeAtPath(path: number[], updater: (node: SAExpressionNode) => SAExpressionNode): void {
    const expression = this.expression$.getValue();
    if (!expression?.Root) return;

    const newRoot = this.updateNodeRecursive(expression.Root, path, 0, updater);
    const newExpression: SAExpression = { ...expression, Root: newRoot };
    this.expression$.next(newExpression);
    this.onExpressionChanged.emit(newExpression);
  }

  private updateNodeRecursive(
    node: SAExpressionNode,
    path: number[],
    depth: number,
    updater: (node: SAExpressionNode) => SAExpressionNode
  ): SAExpressionNode {
    if (depth === path.length) {
      return updater(node);
    }

    const index = path[depth];
    const args = node.ListArgument ? [...node.ListArgument] : [];
    if (index < args.length) {
      args[index] = this.updateNodeRecursive(args[index], path, depth + 1, updater);
    }

    return { ...node, ListArgument: args };
  }

  /** Change function name at path */
  onFunctionChange(path: number[], functionName: string): void {
    this.updateNodeAtPath(path, (node) => {
      const newNode = this.createNode(functionName);
      // Try to preserve existing arguments if compatible
      if (node.ListArgument && newNode.ListArgument) {
        const minLen = Math.min(node.ListArgument.length, newNode.ListArgument.length);
        for (let i = 0; i < minLen; i++) {
          newNode.ListArgument[i] = node.ListArgument[i];
        }
      }
      return newNode;
    });
  }

  /** Change value text at path */
  onValueChange(path: number[], value: string): void {
    this.updateNodeAtPath(path, (node) => ({ ...node, ValueText: value }));
  }

  /** Change reference at path */
  onReferenceChange(path: number[], refType: string, refId: string): void {
    this.updateNodeAtPath(path, (node) => ({
      ...node,
      ReferenceType: refType,
      ReferenceId: refId,
    }));
  }

  /** Add argument to node at path */
  addArgument(path: number[]): void {
    this.updateNodeAtPath(path, (node) => {
      const args = node.ListArgument ? [...node.ListArgument] : [];
      args.push(this.createNode('number'));
      return { ...node, ListArgument: args };
    });
  }

  /** Remove argument at index from node at path */
  removeArgument(path: number[], argIndex: number): void {
    this.updateNodeAtPath(path, (node) => {
      const args = node.ListArgument ? [...node.ListArgument] : [];
      const minArgs = this.getMinArgs(node.FunctionName);
      if (args.length > minArgs) {
        args.splice(argIndex, 1);
      }
      return { ...node, ListArgument: args };
    });
  }

  /** Handle drag and drop of arguments */
  onArgumentDrop(path: number[], event: CdkDragDrop<SAExpressionNode[]>): void {
    this.updateNodeAtPath(path, (node) => {
      const args = node.ListArgument ? [...node.ListArgument] : [];
      moveItemInArray(args, event.previousIndex, event.currentIndex);
      return { ...node, ListArgument: args };
    });
  }

  /** Check if node can add more arguments */
  canAddArgument(node: SAExpressionNode): boolean {
    return this.isVariableArgs(node.FunctionName);
  }

  /** Check if node can remove arguments */
  canRemoveArgument(node: SAExpressionNode): boolean {
    const minArgs = this.getMinArgs(node.FunctionName);
    return (node.ListArgument?.length ?? 0) > minArgs;
  }

  /** Get display label for a node */
  getNodeLabel(node: SAExpressionNode): string {
    const def = this.getFunctionDef(node.FunctionName);
    return def?.label ?? node.FunctionName ?? 'Unknown';
  }

  /** Check if node is a value node (leaf) */
  isValueNode(node: SAExpressionNode): boolean {
    const category = this.getFunctionDef(node.FunctionName)?.category;
    return category === 'value' || category === 'reference';
  }
}

export function evaluateExpression(expression: SAExpression, lookup: (id: string) => any): any {
  if (expression.Root) {
    return evaluateExpressionNode(expression.Root, lookup);
  }
  return null;
}

export function evaluateExpressionNode(
  node: SAExpressionNode,
  lookup: (id: string) => any
): any {
  const functionName = node.FunctionName;

  // Handle value types (leaf nodes)
  if (functionName === 'string') {
    return node.ValueText ?? '';
  }
  if (functionName === 'number') {
    return parseFloat(node.ValueText ?? '0') || 0;
  }
  if (functionName === 'boolean') {
    return node.ValueText === 'true';
  }

  // Handle reference
  if (functionName === 'reference') {
    const refId = node.ReferenceId;
    if (refId) {
      return lookup(refId);
    }
    return null;
  }

  // Evaluate arguments
  const args = node.ListArgument?.map(arg => evaluateExpressionNode(arg, lookup)) ?? [];

  // Math operations
  if (functionName === 'add') {
    return args.reduce((sum, val) => sum + (Number(val) || 0), 0);
  }
  if (functionName === 'subtract') {
    if (args.length < 2) return 0;
    return (Number(args[0]) || 0) - (Number(args[1]) || 0);
  }
  if (functionName === 'multiply') {
    return args.reduce((product, val) => product * (Number(val) || 0), 1);
  }
  if (functionName === 'divide') {
    if (args.length < 2) return 0;
    const divisor = Number(args[1]) || 0;
    if (divisor === 0) return 0; // Avoid division by zero
    return (Number(args[0]) || 0) / divisor;
  }

  // Comparison operations
  if (functionName === 'equals') {
    if (args.length < 2) return false;
    return args[0] === args[1];
  }
  if (functionName === 'greater') {
    if (args.length < 2) return false;
    return (Number(args[0]) || 0) > (Number(args[1]) || 0);
  }
  if (functionName === 'less') {
    if (args.length < 2) return false;
    return (Number(args[0]) || 0) < (Number(args[1]) || 0);
  }
  if (functionName === 'greaterorequal') {
    if (args.length < 2) return false;
    return (Number(args[0]) || 0) >= (Number(args[1]) || 0);
  }
  if (functionName === 'lessorequal') {
    if (args.length < 2) return false;
    return (Number(args[0]) || 0) <= (Number(args[1]) || 0);
  }

  // Logical operations
  if (functionName === 'and') {
    return args.every(val => Boolean(val));
  }
  if (functionName === 'or') {
    return args.some(val => Boolean(val));
  }
  if (functionName === 'not') {
    return !Boolean(args[0]);
  }

  // String operations
  if (functionName === 'startswith') {
    if (args.length < 2) return false;
    return String(args[0]).startsWith(String(args[1]));
  }
  if (functionName === 'contains') {
    if (args.length < 2) return false;
    return String(args[0]).includes(String(args[1]));
  }
  if (functionName === 'concat') {
    return args.map(val => String(val ?? '')).join('');
  }

  return null;
}

export type EvaluationResult={
  value:any;
  error:string|undefined;
}
export type EvaluationFunction = (lookup: (id: string) => any) => EvaluationResult;
export function generateFunctionForExpression(expression: SAExpression): EvaluationFunction {
  if (!expression.Root) {
    return () => ({ value: null, error: 'No root node' });
  }
  return generateFunctionForExpressionNode(expression.Root)
}
export function generateFunctionForExpressionNode(node: SAExpressionNode): EvaluationFunction {
  const functionName = node.FunctionName;

  // Handle value types (leaf nodes)
if (functionName === 'string') {
    return () => ({ value: node.ValueText ?? '', error: undefined });
  }
  if (functionName === 'number') {
    return () => ({ value: parseFloat(node.ValueText ?? '0') || 0, error: undefined });
  }
  if (functionName === 'boolean') {
    return () => ({ value: node.ValueText === 'true', error: undefined });
  }
  if (functionName === 'reference') {
    const refId = node.ReferenceId;
    if (refId) {
      return (lookup: (id: string) => any) => ({ value: lookup(refId), error: undefined });
    }
    return () => ({ value: null, error: 'No reference id' });
  }
  if (functionName === 'add') {
    const args = node.ListArgument?.map(generateFunctionForExpressionNode) ?? [];
    return (lookup: (id: string) => any) => ({ value: args.reduce((sum, val) => sum + (Number(val(lookup).value) || 0), 0), error: undefined });
  }
  if (functionName === 'subtract') {
    const args = node.ListArgument?.map(generateFunctionForExpressionNode) ?? [];
    return (lookup: (id: string) => any) => ({ value: (Number(args[0](lookup).value) || 0) - (Number(args[1](lookup).value) || 0), error: undefined });
  }
  if (functionName === 'multiply') {
    const args = node.ListArgument?.map(generateFunctionForExpressionNode) ?? [];
    return (lookup: (id: string) => any) => ({ value: args.reduce((product, val) => product * (Number(val(lookup).value) || 0), 1), error: undefined });
  }
  if (functionName === 'divide') {
    const args = node.ListArgument?.map(generateFunctionForExpressionNode) ?? [];
    return (lookup: (id: string) => any) => {
      const divisor = Number(args[1](lookup).value) || 0;
      if (divisor === 0) return { value: 0, error: 'Division by zero' };
      return { value: (Number(args[0](lookup).value) || 0) / divisor, error: undefined };
    };
  }
  if (functionName === 'equals') {
    const args = node.ListArgument?.map(generateFunctionForExpressionNode) ?? [];
    return (lookup: (id: string) => any) => ({ value: args[0](lookup).value === args[1](lookup).value, error: undefined });
  }
  if (functionName === 'greater') {
    const args = node.ListArgument?.map(generateFunctionForExpressionNode) ?? [];
    return (lookup: (id: string) => any) => ({ value: (Number(args[0](lookup).value) || 0) > (Number(args[1](lookup).value) || 0), error: undefined });
  }
  if (functionName === 'less') {
    const args = node.ListArgument?.map(generateFunctionForExpressionNode) ?? [];
    return (lookup: (id: string) => any) => ({ value: (Number(args[0](lookup).value) || 0) < (Number(args[1](lookup).value) || 0), error: undefined });
  }
  if (functionName === 'greaterorequal') {
    const args = node.ListArgument?.map(generateFunctionForExpressionNode) ?? [];
    return (lookup: (id: string) => any) => ({ value: (Number(args[0](lookup).value) || 0) >= (Number(args[1](lookup).value) || 0), error: undefined });
  }
  if (functionName === 'lessorequal') {
    const args = node.ListArgument?.map(generateFunctionForExpressionNode) ?? [];
    return (lookup: (id: string) => any) => ({ value: (Number(args[0](lookup).value) || 0) <= (Number(args[1](lookup).value) || 0), error: undefined });
  }
  if (functionName === 'and') {
    const args = node.ListArgument?.map(generateFunctionForExpressionNode) ?? [];
    return (lookup: (id: string) => any) => ({ value: args.every(val => Boolean(val(lookup).value)), error: undefined });
  }
  if (functionName === 'or') {
    const args = node.ListArgument?.map(generateFunctionForExpressionNode) ?? [];
    return (lookup: (id: string) => any) => ({ value: args.some(val => Boolean(val(lookup).value)), error: undefined });
  }
  if (functionName === 'not') {
    const args = node.ListArgument?.map(generateFunctionForExpressionNode) ?? [];
    return (lookup: (id: string) => any) => ({ value: !Boolean(args[0](lookup).value), error: undefined });
  }
  if (functionName === 'startswith') {
    const args = node.ListArgument?.map(generateFunctionForExpressionNode) ?? [];
    return (lookup: (id: string) => any) => ({ value: String(args[0](lookup).value).startsWith(String(args[1](lookup).value)), error: undefined });
  }
  if (functionName === 'contains') {
    const args = node.ListArgument?.map(generateFunctionForExpressionNode) ?? [];
    return (lookup: (id: string) => any) => ({ value: String(args[0](lookup).value).includes(String(args[1](lookup).value)), error: undefined });
  }
  if (functionName === 'concat') {
    const args = node.ListArgument?.map(generateFunctionForExpressionNode) ?? [];
    return (lookup: (id: string) => any) => ({ value: args.map(val => String(val(lookup).value ?? '')).join(''), error: undefined });
  }
  //
  return (lookup: (id: string) => any) => ({ value: null, error: "unknown function" });
}