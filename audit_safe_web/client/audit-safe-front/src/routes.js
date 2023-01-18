import LoginPage from "./pages/LoginPage";
import {LOGIN, MAIN} from "./utils/const";
import MainPage from "./pages/MainPage";

export const authRoutes = [
    {
        path: MAIN,
        component: <MainPage/>
    }
]

export const publicRoutes = [
    {
        path: LOGIN,
        component: <LoginPage/>
    }
]