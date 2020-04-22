
const textUtils = {
    removeExcessSpace(str: string) {
        str = str.trim();
        str = str.replace(/\s{2,}/, s => s.match(/\n/) ? '\n' : ' ');
        return str;
    },

    getFirstWords(str: string, numWords: number) {
        let i = 0;
        let nw = 0;
        
        // get to the end of the n'th word
        for (i = 0; i < str.length - 1; i++) {
            if (!/\s/.test(str[i]) && /\s/.test(str[i + 1])) {
                if (++nw >= numWords) {
                    break;
                }
            }
        }
        
        // get to the end of the sentence
        while (i < str.length - 1 && !(str[i] == '.' && /\s/.test(str[i + 1]))) {
            i++;
        }

        let ss = str.slice(0, i + 1);
        return ss.trim();
    }
};

export default textUtils;
