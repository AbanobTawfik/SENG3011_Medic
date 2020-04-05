
import * as util from 'util';

import * as moment from 'moment';

import articleStore from '../interfaces/articleStore';

const requests = articleStore.createRequests(
    moment.utc([2020, 0, 1, 0, 0, 0]),
    moment.utc([2020, 1, 1, 0, 0, 0]),
    [], '',
    []);

requests.forEach(req => {
    req.fetchAll().then(res => {
        console.log(
            'Results:',
            {
                'API': req.getApiName(),
                'Number of articles': res.length,
            }
        );

        // res contains all articles satisfying the request from one API
        // in StandardArticle form

        // if (res.length > 0) {
        //     console.log('First article:');
        //     console.log(util.inspect(res[0], false, null, true));
        //     if (res.length > 1) {
        //         console.log('Last article:');
        //         console.log(util.inspect(res[res.length - 1],
        //                                  false, null, true));
        //     }
        // }
    });
});
