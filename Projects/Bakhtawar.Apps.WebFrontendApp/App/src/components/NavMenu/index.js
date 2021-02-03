import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler } from 'reactstrap';
import styled from 'styled-components';
import { AccountMenu } from './components/AccountMenu';
import { ContentMenu } from './components/ContentMenu';

import './index.css';

import icon from 'assets/icon.png';

const NavbarLogo = styled.img.attrs(() => ({
  className: 'navbar-logo',
  src: icon,
  alt: 'Bakhtawar Logo',
  width: 30,
  height: 30
}))`
  margin-right: 6px;
`;

export const NavMenu = () => {
  const [isCollapsed, setIsCollapsed] = useState(true);

  const toggleNavbar = () => {
    setIsCollapsed((isCollapsed) => !isCollapsed);
  };

  return (
    <Navbar className="bg-dark" expand="lg" dark fixed="top" id="main-nav">
      <Container>
        <NavbarBrand tag={Link} to="#app">
          <NavbarLogo/>
          Bakhtawar
        </NavbarBrand>
        <NavbarToggler onClick={toggleNavbar} className="mr-2"/>
        <Collapse isOpen={!isCollapsed} navbar>
          <ContentMenu/>
          <AccountMenu/>
        </Collapse>
      </Container>
    </Navbar>
  );
};
