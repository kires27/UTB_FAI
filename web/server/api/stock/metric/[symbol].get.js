import { supabaseClientAuth } from '~~/server/utils/supabase/auth';

import { getStock, getMetric } from "#shared/query/stock";

export default defineEventHandler(async (event) => {
    const { client } = await supabaseClientAuth(event, false);
    const symbol = getRouterParam(event, "symbol");
    const { select } = getQuery(event);
    const columns = select ? select.split(',') : ["*"];

    const stock = await getStock(client, symbol, ["id"]);
    const id = stock.id;
    const data = await getMetric(client, id, columns);

    if (!data) throw createError({ statusCode: 404, statusMessage: "No metrics found" });

    return data;
})
