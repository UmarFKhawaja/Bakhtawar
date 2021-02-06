import React from 'react';
import { useContent } from 'components/Content';
import { Layout } from 'components/Layout';
import { Footer } from './components/Footer';
import { Header } from './components/Header';
import { Section } from './components/Section';

import react from 'assets/react.png';

export const Home = () => {
  const content = useContent();

  return (
    <>
      <Layout>
        <Header/>
        <Section light>
          <h3 className="text-muted">Powered by</h3>
          <h1>
            <a href="https://reactjs.org" className="text-decoration-none" target="_blank" rel="noreferrer">
              <img src={react} alt="React"/>
              <span className="text-dark">React</span>
            </a>
          </h1>
          <h5>
            v17.0.1
          </h5>
        </Section>
        {
          content.map((section, index) => {
            const { path, component: Component } = section;

            return (
              <Section key={path} id={path} light={index % 2 !== 0}>
                <Component/>
              </Section>
            )
          })
        }
        <Footer/>
      </Layout>
    </>
  );
};
