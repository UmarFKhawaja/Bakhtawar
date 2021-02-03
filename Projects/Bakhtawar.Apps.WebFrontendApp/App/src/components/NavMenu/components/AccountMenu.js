import React, { useEffect, useState } from 'react';
import { NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import { ApplicationPaths } from 'constants/Authorization';
import { authorizationManager } from 'services/authorization-manager';

const AuthenticatedView = ({ logoutPath }) => {
  return (
    <>
      <NavItem>
        <NavLink tag={Link} className="text-light" to={logoutPath}>Logout</NavLink>
      </NavItem>
    </>
  );
};

const UnauthenticatedView = ({ registerPath, loginPath }) => {
  return (
    <>
      <NavItem>
        <NavLink tag={Link} className="text-light" to={registerPath}>Register</NavLink>
      </NavItem>
      <NavItem>
        <NavLink tag={Link} className="navbar-light" to={loginPath}>Login</NavLink>
      </NavItem>
    </>
  );
};

export const AccountMenu = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [userName, setUserName] = useState(null);

  const populateState = async () => {
    const [
      isAuthenticated,
      user
    ] = await Promise.all([
      authorizationManager.isAuthenticated(),
      authorizationManager.getUser()
    ]);
    
    setIsAuthenticated(isAuthenticated);
    setUserName(user && user.name);
  };

  useEffect(() => {
    const subscription = authorizationManager.subscribe(populateState);

    populateState().then();

    return () => {
      authorizationManager.unsubscribe(subscription);
    };
  }, []);

  return (
    <ul className="navbar-nav ml-auto">
      {
        isAuthenticated
          ? (
            <AuthenticatedView userName={userName} profilePath={`${ApplicationPaths.Profile}`} logoutPath={{ pathname: `${ApplicationPaths.LogOut}`, state: { local: true } }}/>
          )
          : (
            <UnauthenticatedView registerPath={`${ApplicationPaths.Register}`} loginPath={`${ApplicationPaths.Login}`}/>
          )
      }
    </ul>
  );
};
