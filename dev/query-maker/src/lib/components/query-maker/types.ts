export type RuleType = 'single' | 'group';
export type GroupOperator = 'and' | 'or';

export interface BaseRule {
    id: string;
    type: RuleType;
}

export interface SingleRule extends BaseRule {
    type: 'single';
    name: string;
    value: string;
}

export interface GroupRule extends BaseRule {
    type: 'group';
    operator: GroupOperator;
    rules: Rule[];
}

export type Rule = SingleRule | GroupRule;