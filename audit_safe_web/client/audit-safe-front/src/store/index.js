import {configureStore} from "@reduxjs/toolkit";
import user from './userSlice'
import data from './dataSlice'

export default configureStore({
    reducer: {
        user,
        data,
    }
})