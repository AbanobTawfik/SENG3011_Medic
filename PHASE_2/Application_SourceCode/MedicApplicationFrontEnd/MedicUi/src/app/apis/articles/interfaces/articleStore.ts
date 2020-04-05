
import * as moment from 'moment';

import ArticleRequest from '../request/ArticleRequest';

import ApiDemic from '../concrete/ApiDemic';
import CalmClams from '../concrete/CalmClams';
import Codeonavirus from '../concrete/Codeonavirus';
import ExceptionalCoders from '../concrete/ExceptionalCoders';
import FlyingSplaucers from '../concrete/FlyingSplaucers';
import Fm from '../concrete/Fm';
import GnarlyNarwhals from '../concrete/GnarlyNarwhals';
import Medics from '../concrete/Medics';
import Webbscrapers from '../concrete/Webbscrapers';

const API_MAP = {
    'ApiDemic': ApiDemic,
    'CalmClams': CalmClams,
    'Codeonavirus': Codeonavirus,
    'ExceptionalCoders': ExceptionalCoders,
    'FlyingSplaucers': FlyingSplaucers,
    'Fm': Fm,
    'GnarlyNarwhals': GnarlyNarwhals,
    'Medics': Medics,
    'Webbscrapers': Webbscrapers,
};

const articleStore = {
     /**
      * Returns an array of requests, one for each specified API
      * @param startDate - the start date in UTC time
      * @param endDate   - the end date in UTC time
      * @param keyTerms  - an array of key terms
      * @param location  - a location
      * @param apis      - an array of names of the APIs to be used.
      *                    Leave this blank to use all APIs.
      */
    createRequests: function(startDate: moment.Moment,
                             endDate:   moment.Moment,
                             keyTerms:  string[],
                             location:  string,
                             apis:      string[] = []): ArticleRequest[]
    {
        if (apis.length == 0) {
            apis = Object.keys(API_MAP);
        }
        
        return apis
            .filter(name => name in API_MAP)
            .map(name =>
                new ArticleRequest(
                    startDate, endDate, keyTerms, location,
                    new API_MAP[name]()
                )
            );
        
    }
}

export default articleStore;
