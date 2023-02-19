import {BrowserRouter as Router} from "react-router-dom";
import AppRouter from "./components/AppRouter";
import {useDispatch, useSelector} from "react-redux";
import {check} from "./http/userAPI";
import {setIsAuth, setUser} from "./store/userSlice";
import {useEffect, useState} from "react";
import {Box, CircularProgress} from "@mui/material";

function App() {
    const dispatch = useDispatch()

    const[loading, setLoading] = useState(true)

    useEffect(() => {
        check().then(data => {
            dispatch(setUser(data))
            dispatch(setIsAuth(true))
        }).finally(()=> setLoading(false))
    },[])

    if (loading){
        return <Box sx={{display: 'flex', justifyContent: 'center', alignItems: 'center', justify: 'center', marginTop: 80}}><CircularProgress size={150}/></Box>
    }

  return (
    <Router>
      <AppRouter/>
    </Router>
  );
}

export default App;
