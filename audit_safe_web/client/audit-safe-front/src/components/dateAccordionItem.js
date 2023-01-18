import {Accordion, AccordionDetails, AccordionSummary, Stack, Typography} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import Divider from "@mui/material/Divider";
import {useEffect, useState} from "react";

const DateAccordionItem = ({data}) =>{

    const options = { year: 'numeric', month: 'numeric', day: 'numeric', hour: 'numeric',minute:'numeric' };
    const date = new Date(data.date)


    const [colors, setColors] = useState({})
    useEffect(() => {
        if(data.type==="Red Alert!"){
            setColors({
                header: "#ff686b",
                main: "#fcd5ce",
            })
        }

        if(data.type==="Warning"){
            setColors({
                header: "#fec89a",
                main: "#f9dcc4",
            })
        }
        if(data.type==="Незначительно"){
            setColors({
                header: "#e9edc9",
                main: "#fefae0",
            })
        }
    },[])




    return(
    <Accordion>
        <AccordionSummary style={{width: 1200, backgroundColor: colors.header}} id='panel1-header' aria-controls='panel1-content' expandIcon={<ExpandMoreIcon/>}>
            <Stack ml={4} direction="row" style={{width: "90%"}} justifyContent="space-between" spacing={7} divider={<Divider orientation="vertical" flexItem />}>
                <Typography variant="h6" gutterBottom>{data.domain}</Typography>
                <Typography variant="h6" gutterBottom>{data.user}</Typography>
                <Typography variant="h6" gutterBottom>{data.type}</Typography>
                <Typography variant="h6" gutterBottom>{date.toLocaleDateString('ru-RU', options)}</Typography>
            </Stack>

        </AccordionSummary>
        <AccordionDetails style={{width: 1168,backgroundColor: colors.main}}>
            <Typography mt={3}  ml={4} variant="subtitle1" gutterBottom>
                {data.message}
            </Typography>
        </AccordionDetails>
    </Accordion>
    )
}

export default DateAccordionItem;
