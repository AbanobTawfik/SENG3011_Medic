
import * as util from 'util';

import articleStore from '../interfaces/articleStore';

const requests = articleStore.createRequests(
    new Date(2020, 0, 1, 0, 0, 0),
    new Date(2020, 3, 4, 0, 0, 0),
    ['infection', 'outbreak'], '',
    ['Medics', 'Codeonavirus']);

requests.forEach(req => req.fetchFirst().then(
    res => console.log(util.inspect(res, false, null, true))
));
