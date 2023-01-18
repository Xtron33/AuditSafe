import {Stack, TextField, Button, Typography, Accordion, AccordionSummary, AccordionDetails} from "@mui/material";
import {useEffect, useState} from "react";
import Divider from '@mui/material/Divider';
import DateAccordionItem from "../components/dateAccordionItem";
import {useDispatch, useSelector} from "react-redux";
import {fetchData} from "../http/dataAPI";
import {setData} from "../store/dataSlice";
import Skeleton from '@mui/material/Skeleton';


const MainPage = () => {
    const[loading, setLoading] = useState(true)


    const dispatch = useDispatch()
    const data = useSelector((state) => state.data.data)

    useEffect(() => {
        fetchData().then(data => dispatch(setData(data))).finally(() => setLoading(false))
    },[])


    return(
        <Stack  alignItems="center" spacing={0}>
            <Typography mt={8} mb={8} variant="h4" gutterBottom>
                AuditSafe
            </Typography>

            <Accordion disabled>
                <AccordionSummary style={{width: 1000, backgroundColor: "black"}}
                    aria-controls="panel3a-content"
                    id="panel3a-header"
                >
                    <Stack style={{width: "86.5%"}} justifyContent="space-between" ml={4} direction="row" spacing={7} divider={<Divider color="white" orientation="vertical" flexItem />}>
                        <Typography color="white" variant="h6" gutterBottom>Domain</Typography>
                        <Typography color="white" variant="h6" gutterBottom>User</Typography>
                        <Typography color="white" variant="h6" gutterBottom>Type</Typography>
                    </Stack>
                </AccordionSummary>
            </Accordion>

            {loading ?  <Skeleton variant="rectangular" width={210} height={118} /> : data.map(data => <DateAccordionItem key={data.id} data={data}/> )};
        </Stack>
    )
}

export default MainPage;