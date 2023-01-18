import {Stack, TextField, Button, Typography} from "@mui/material";
import {useDispatch, useSelector} from "react-redux";
import {sigin} from "../http/userAPI";
import {setIsAuth, setUser} from "../store/userSlice";
import {useState} from "react";
import {useNavigate} from "react-router-dom"
import {MAIN} from "../utils/const";

const LoginPage = () => {
    const dispatch = useDispatch()
    const isAuth = useSelector((state) => state.user.isAuth)
    const user = useSelector((state) => state.user.user)

    const navigate = useNavigate();

    const [login, setLogin] = useState('')
    const [password, setPassword] = useState('')

    const logIn = async () => {
        try{
            let data;
            data = await sigin(login,password)
            dispatch(setUser(data))
            dispatch(setIsAuth(true));

            navigate(MAIN)
            console.log(isAuth)
        }
        catch (e){
            alert(e.response.data.message)
        }
    }

    return(
        <>
            <Stack style={{height: "100vh"}} justifyContent="center"  alignItems="center" spacing={0}>
                <Typography variant="h1" gutterBottom>
                    AuditSafe
                </Typography>
                <TextField value={login} onChange={e => setLogin(e.target.value)}  style={{width: 500}} size="normal" id="login" label="Login" variant="filled" />
                <TextField value={password} onChange={e => setPassword(e.target.value)} type="password"  style={{width: 500}} size="normal" id="password" label="Password" variant="filled" />
                <Button onClick={logIn} style={{width: 500, marginTop: 50}} variant="contained" >Sig in</Button>
            </Stack>
        </>
    )
}

export default LoginPage;