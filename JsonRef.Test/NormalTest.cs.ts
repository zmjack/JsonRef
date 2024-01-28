function eval_jsonref_refs(
    $refs: Record<string, {
        $type: string,
        $: any,
    }>
): Record<string, any> {
    let pool: Record<string, any> = {};
    for (let key in $refs) {
        pool[key] = { ...$refs[key].$ };
    }

    function eval_value(value: any): any {
        if (Array.isArray(value)) {
            return [...value.map(eval_value)];
        } else if (typeof value === 'object') {
            if (value['$ref']) {
                return pool[value['$ref']];
            }

            for (let key in value) {
                value[key] = eval_value(value[key]);
            }
            return value;
        } else {
            return value;
        }
    }

    for (let key in pool) {
        let value = pool[key];
        pool[key] = eval_value(value);
    }

    return pool;
}

function eval_jsonref<T>(jsonref: {
    $refs: Record<string, {
        $type: string,
        $: any,
    }>,
    $: any,
}): T {
    const pool = eval_jsonref_refs(jsonref.$refs);
    return pool[jsonref.$.$ref];
}
