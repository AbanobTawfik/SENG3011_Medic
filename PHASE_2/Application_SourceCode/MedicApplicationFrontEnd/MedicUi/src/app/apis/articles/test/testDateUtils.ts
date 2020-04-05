
import * as moment from 'moment';

import dateUtils from '../utils/dateUtils';

console.log(dateUtils.xStrToDateRange('2018-01-03 xx:xx:xx'));

console.log(dateUtils.xStrToDateRange('2018-01-03 xx:xx:xx to ' +
                                      '2020-02-xx xx:xx:xx'));

console.log(dateUtils.xStrToDateRange('2018-01-01 12:00:00'));

console.log(dateUtils.xStrToDateRange('2018-xx-xx xx:xx:xx to ' +
                                      '2019-xx-xx xx:xx:xx'));

console.log(moment.utc([2011, 0, 1, 8, 30, 55]).format());
