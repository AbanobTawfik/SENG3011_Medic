#!/bin/sh

BLANK="(blank)"

################################################################################

main()
{
    script_dir="$(dirname "$(readlink -f $0)")"
    inputs_dir="$script_dir/Inputs/"
    output_dir="$script_dir/Expected/"

    mkdir -p "$output_dir"

    for input_file in "$inputs_dir"/*.in
    do
        reset_params
        source $input_file
        set_query_string

        output_basename="$output_dir/$(basename "$input_file" .in).out"
        curl -X GET "http://localhost:5000/api/Reports/GetArticles" --data-urlencode "$query" -H "accept: */*" | python -m json.tool > "$output_basename"
    done
}

################################################################################

reset_params()
{
    start_date=$BLANK
    end_date=$BLANK
    timezone=$BLANK
    key_terms=$BLANK
    location=$BLANK
    max=$BLANK
    offset=$BLANK
}

################################################################################

set_query_string()
{
    # build query string one parameter at a time
    query=""

    if [ "$start_date" != "$BLANK" ]
    then
        query="$query&start_date=$start_date"
    fi

    if [ "$end_date" != "$BLANK" ]
    then
        query="$query&end_date=$end_date"
    fi

    if [ "$timezone" != "$BLANK" ]
    then
        query="$query&timezone=$timezone"
    fi

    if [ "$key_terms" != "$BLANK" ]
    then
        query="$query&key_terms=$key_terms"
    fi

    if [ "$max" != "$BLANK" ]
    then
        query="$query&max=$max"
    fi

    if [ "$offset" != "$BLANK" ]
    then
        query="$query&offset=$offset"
    fi

    # chop off the first '&'
    query="$(echo "$query" | sed -e 's/^&//')"
}

################################################################################

main
