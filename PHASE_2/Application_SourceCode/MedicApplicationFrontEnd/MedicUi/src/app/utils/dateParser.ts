
import * as moment from 'moment';

function xsToDateRange(y1s: string, M1s: string, d1s: string,
                       H1s: string, m1s: string, s1s: string,
                       y2s: string, M2s: string, d2s: string,
                       H2s: string, m2s: string, s2s: string)
{
    const y1 = parseInt(y1s);
    const y2 = parseInt(y2s);

    const M1 = (M1s == 'xx' ?  1 : parseInt(M1s));
    const M2 = (M2s == 'xx' ? 12 : parseInt(M2s));

    const dm = daysInMonth(M2, y2);

    const d1 = (d1s == 'xx' ?  1 : parseInt(d1s));
    const d2 = (d2s == 'xx' ? dm : parseInt(d2s));

    const H1 = (H1s == 'xx' ?  0 : parseInt(H1s));
    const H2 = (H2s == 'xx' ? 23 : parseInt(H2s));

    const m1 = (m1s == 'xx' ?  0 : parseInt(m1s));
    const m2 = (m2s == 'xx' ? 59 : parseInt(m2s));

    const s1 = (s1s == 'xx' ?  0 : parseInt(s1s));
    const s2 = (s2s == 'xx' ? 59 : parseInt(s2s));

    return [
        moment.utc([y1, M1 - 1, d1, H1, m1, s1]),
        moment.utc([y2, M2 - 1, d2, H2, m2, s2]),
    ];
}

function daysInMonth(m: number, y: number)
{
    switch (m)
    {
        case  1: return 31;
        case  2: return isLeapYear(y) ? 29 : 28;
        case  3: return 31;
        case  4: return 30;
        case  5: return 31;
        case  6: return 30;
        case  7: return 31;
        case  8: return 31;
        case  9: return 30;
        case 10: return 31;
        case 11: return 30;
        case 12: return 31;
        default: return  0;
    }
}

function isLeapYear(y: number)
{
    return y % 400 == 0 || (y % 100 != 0 && y % 4 == 0);
}

function xsToDateFloor(ys: string,
                       Ms: string = 'xx',
                       ds: string = 'xx',
                       Hs: string = 'xx',
                       ms: string = 'xx',
                       ss: string = 'xx')
{
    const y = parseInt(ys);
    const M = (Ms == 'xx' ?  1 : parseInt(Ms));
    const d = (ds == 'xx' ?  1 : parseInt(ds));
    const H = (Hs == 'xx' ?  0 : parseInt(Hs));
    const m = (ms == 'xx' ?  0 : parseInt(ms));
    const s = (ss == 'xx' ?  0 : parseInt(ss));
    return moment.utc([y, M - 1, d, H, m, s]);
}

////////////////////////////////////////////////////////////////////////

const dateUtils = {
    xStrToDateRange(str: string)
    {
        if (str == null) return null;

        const reExact = /(\d{4})-(\d{2}|xx)-(\d{2}|xx) (\d{2}|xx):(\d{2}|xx):(\d{2}|xx)/;
        const reRange = /(\d{4})-(\d{2}|xx)-(\d{2}|xx) (\d{2}|xx):(\d{2}|xx):(\d{2}|xx) *to *(\d{4})-(\d{2}|xx)-(\d{2}|xx) (\d{2}|xx):(\d{2}|xx):(\d{2}|xx)/;

        const match1 = str.match(reExact);
        const match2 = str.match(reRange);

        if (match2) {
            const y1 = match2[ 1];
            const M1 = match2[ 2];
            const d1 = match2[ 3];
            const H1 = match2[ 4];
            const m1 = match2[ 5];
            const s1 = match2[ 6];
            
            const y2 = match2[ 7];
            const M2 = match2[ 8];
            const d2 = match2[ 9];
            const H2 = match2[10];
            const m2 = match2[11];
            const s2 = match2[12];

            return xsToDateRange(
                y1, M1, d1, H1, m1, s1,
                y2, M2, d2, H2, m2, s2,
            );
        } else if (match1) {
            const y1 = match1[ 1];
            const M1 = match1[ 2];
            const d1 = match1[ 3];
            const H1 = match1[ 4];
            const m1 = match1[ 5];
            const s1 = match1[ 6];

            return xsToDateRange(
                y1, M1, d1, H1, m1, s1,
                y1, M1, d1, H1, m1, s1,
            );
        } else {
            return null;
        }
    },

    xStrToDateFloor(str: string): moment.Moment
    {
        if (str == null) return null;

        const reExact1 = /(\d{4})-(\d{2}|xx)-(\d{2}|xx)(?: |T)(\d{2}|xx):(\d{2}|xx):(\d{2}|xx)/;
        const reExact2 = /(\d{4})-(\d{2}|xx)-(\d{2}|xx)/;
        
        if (reExact1.test(str)) {
            const match = str.match(reExact1);
            const y = match[1];
            const M = match[2];
            const d = match[3];
            const H = match[4];
            const m = match[5];
            const s = match[6];
            return xsToDateFloor(y, M, d, H, m, s);

        } else if (reExact2.test(str)) {
            const match = str.match(reExact2);
            const y = match[1];
            const M = match[2];
            const d = match[3];
            return xsToDateFloor(y, M, d);
        
        } else {
            console.log('BAD DATE: '+ str);
            return null;
        }
    }
}

export default dateUtils;
