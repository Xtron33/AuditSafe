import {$authHost, $host} from "./index";

export const fetchData = async () => {
    const {data} = await $authHost.get('api/data/')
    return data
}