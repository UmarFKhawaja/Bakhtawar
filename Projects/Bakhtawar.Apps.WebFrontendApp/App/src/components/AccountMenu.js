import React, { useEffect, useState } from 'react';
import { NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import { ApplicationPaths } from 'constants/Authorization';
import { authorizationManager } from 'services/authorization-manager';

const AuthenticatedView = ({ userName, profilePath, logoutPath }) => {
  return (
    <>
      <NavItem>
        <NavLink tag={Link} className="text-dark" to={profilePath}>Hello {userName}</NavLink>
      </NavItem>
      <NavItem>
        <NavLink tag={Link} className="text-dark" to={logoutPath}>Logout</NavLink>
      </NavItem>
    </>
  );
};

const UnauthenticatedView = ({ registerPath, loginPath }) => {
  return (
    <>
      <NavItem>
        <NavLink tag={Link} className="text-dark" to={registerPath}>Register</NavLink>
      </NavItem>
      <NavItem>
        <NavLink tag={Link} className="text-dark" to={loginPath}>Login</NavLink>
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

  if (!isAuthenticated) {
    const registerPath = `${ApplicationPaths.Register}`;
    const loginPath = `${ApplicationPaths.Login}`;

    return (
      <UnauthenticatedView registerPath={registerPath} loginPath={loginPath}/>
    );
  } else {
    const profilePath = `${ApplicationPaths.Profile}`;
    const logoutPath = { pathname: `${ApplicationPaths.LogOut}`, state: { local: true } };

    return (
      <AuthenticatedView userName={userName} profilePath={profilePath} logoutPath={logoutPath}/>
    );
  }
};
