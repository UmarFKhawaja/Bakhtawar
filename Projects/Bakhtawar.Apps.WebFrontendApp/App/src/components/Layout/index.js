import React from 'react';
import { NavMenu } from 'components/NavMenu';

export const Layout = ({ children }) => (
  <>
    <NavMenu/>
    {children}
  </>
);
