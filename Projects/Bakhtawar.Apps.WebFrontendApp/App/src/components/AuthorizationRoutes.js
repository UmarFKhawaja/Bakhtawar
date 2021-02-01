import React from 'react';
import { Route } from 'react-router';
import { Login } from './Login';
import { Logout } from './Logout';
import { ApplicationPaths, LoginActions, LogoutActions } from '../constants/Authorization';

export const AuthorizationRoutes = () => (
  <>
    <Route path={ApplicationPaths.Login}><Login action={LoginActions.Login}/></Route>
    <Route path={ApplicationPaths.LoginFailed}><Login action={LoginActions.LoginFailed}/></Route>
    <Route path={ApplicationPaths.LoginCallback}><Login action={LoginActions.LoginCallback}/></Route>
    <Route path={ApplicationPaths.Profile}><Login action={LoginActions.Profile}/></Route>
    <Route path={ApplicationPaths.Register}><Login action={LoginActions.Register}/></Route>
    <Route path={ApplicationPaths.LogOut}><Logout action={LogoutActions.Logout}/></Route>
    <Route path={ApplicationPaths.LogOutCallback}><Logout action={LogoutActions.LogoutCallback}/></Route>
    <Route path={ApplicationPaths.LoggedOut}><Logout action={LogoutActions.LoggedOut}/></Route>
  </>
);