import {Routes, Route, Navigate} from "react-router-dom"
import {authRoutes, publicRoutes} from "../routes";
import {useSelector} from "react-redux";


const AppRouter = () => {
    const isAuth = useSelector((state) => state.user.isAuth)

    return(
        <Routes>
            {isAuth && authRoutes.map(({path, component}) => <Route key={path} path={path} element={component} exact></Route>)}}
            {publicRoutes.map(({path, component}) => <Route key={path} path={path} element={component} exact></Route> )}
            {authRoutes.map(({path}) => <Route path={path} element={<Navigate to="/login" replace/>} ></Route>)}
        </Routes>
    )

}

export default AppRouter