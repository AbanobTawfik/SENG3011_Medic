#!/bin/sh

BLANK="(blank)"

################################################################################

main()
{
    script_dir="$(dirname "$(readlink -f $0)")"
    input_dir="$script_dir/Input/"
    expected_dir="$script_dir/Expected/"

    mkdir -p "$output_dir"

    for input_file in "$input_dir"/*.in
    do
        reset_params
        source $input_file
        set_query_string

        output_filename="$expected_dir/$(basename "$input_file" .in).out"
        curl -G "http://localhost:5000/api/Test/GetArticles?$query" -H "accept: */*" | 
            python3 -m json.tool |
            sed -e 's/"accessed_time": ".*"/"accessed_time": "REMOVED"/' > "$output_filename"
    done
}

################################################################################

reset_params()
{
    for param in start_date end_date timezone key_terms location max offset
    do
        eval $param=\$BLANK
    done
}

################################################################################

set_query_string()
{
    # build query string one parameter at a time
    query=""

    for param in start_date end_date timezone key_terms location max offset
    do
        if [ "${!param}" != "$BLANK" ]
        then
            query="$query&$param=$(urlencode "${!param}")"
        fi
    done

    # chop off the first '&'
    query="$(echo "$query" | sed -e 's/^&//')"
}

################################################################################

if ! which urlencode > /dev/null
then
    echo "$0: command 'urlencode' is not installed"
    exit 1
fi

main
